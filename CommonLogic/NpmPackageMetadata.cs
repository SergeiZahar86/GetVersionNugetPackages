using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GetVersionNugetPackages.Models;
using Newtonsoft.Json;

namespace GetVersionNugetPackages.CommonLogic;

/// <summary>
/// Класс для определения метаинформации о Npm пакетах
/// </summary>
public class NpmPackageMetadata
{
    /// <summary>
    /// Определение названий и версий Npm пакетов
    /// </summary>
    /// <param name="projects">Коллекция ProjectFileInfo с информацией о найденных проектах
    /// </param>
    public static async Task GetNpmVersionPackage(List<ProjectInformation> projects)
    {
        foreach (var project in projects)
        {
            using (FileStream fstream = File.OpenRead(project.PackageJsonFileInfo.FullName))
            {
                byte[] buffer = new byte[fstream.Length];
                await fstream.ReadAsync(buffer, 0, buffer.Length);
                string textFromFile = Encoding.Default.GetString(buffer);

                dynamic stuff = JsonConvert.DeserializeObject(textFromFile);

                foreach (var dependency in stuff.dependencies)
                {
                    var npmPackage = new NpmPackage
                    {
                        Name = dependency.Name,
                        Version = dependency.Value.ToString().Replace("^", "").Replace("~", "")
                    };
                    //project.NpmPackages.Add(npmPackage);
                }
            }
        }
    }


    public static async Task NpmGetAndWriteMetadataToFile(List<ProjectInformation> projects,
        string logFile, string npmCmd, string resulFileName, DateTime criticalDate)
    {
        using (StreamWriter writer = new StreamWriter(resulFileName, true))
        {
            writer.WriteLine(string.Empty);
            writer.WriteLine("-----------------------------------");
            writer.WriteLine("--------- Npm Packages ------------");
            writer.WriteLine("-----------------------------------");
        }

        foreach (var project in projects.Where(project => project.PackageJsonFileInfo != null))
        {
            await NpmGetPackageList(logFile, npmCmd, project.PackageJsonFileInfo.DirectoryName);

            // коллекция с версиями
            var packagesWithVersions =
                NpmParseFile(logFile).OrderBy(x => x.Key);

            // коллекция с только одной версией
            var singleVersionPackages =
                packagesWithVersions.Where(x => x.Value.Count == 1).ToList();

            using (StreamWriter writer = new StreamWriter(resulFileName, true))
            {
                writer.WriteLine(string.Empty);
                writer.WriteLine($"-->  {project.ProjectFileInfo.Name}");
                writer.WriteLine(string.Empty);
            }

            NpmGenerateResult(resulFileName, singleVersionPackages,
                npmCmd, criticalDate);

            // коллекция с несколькими версиями
            var multipleVersionPackages =
                packagesWithVersions.Where(x => x.Value.Count > 1).ToList();

            NpmGenerateResult(resulFileName,
                multipleVersionPackages, npmCmd, criticalDate);
        }
    }

    /// <summary>
    /// Запись результирующей коллекции в файл
    /// </summary>
    /// <param name="resulFileName">Имя результирующего файла для пакетов с единственной версией,
    ///     в который пишется наименование, версия и дата версии пакета.</param>
    /// <param name="packagesWithVersions">Коллекция с названиями пакетов и их версиями</param>
    /// <param name="npmCmd">Путь к файлу npm.cmd из nodejs</param>
    /// <param name="criticalDate"></param>
    public static void NpmGenerateResult(string resulFileName,
        List<KeyValuePair<string, List<string>>> packagesWithVersions, string npmCmd,
        DateTime? criticalDate = null)
    {
        using (StreamWriter writer = new StreamWriter(resulFileName, true))
        {
            foreach (var packageWithVersions in packagesWithVersions)
            {
                foreach (string version in packageWithVersions.Value)
                {
                    string date = NpmGetPackageDate(packageWithVersions.Key, version, npmCmd);
                    string[] dates = date.Split(new char[] {'-'});
                    var specifiedDate = new DateTime(Convert.ToInt32(dates[0]),
                        Convert.ToInt32(dates[1]),
                        Convert.ToInt32(dates[2]));
                    if (criticalDate != null && specifiedDate >= criticalDate)
                    {
                        writer.WriteLine($"*  {packageWithVersions.Key} {version} {date}");
                    }
                    else
                    {
                        writer.WriteLine($"{packageWithVersions.Key} {version} {date}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Запись сырой информации о Npm пакетах в файл
    /// </summary>
    /// <param name="outputFileName">Имя временного файла, в который пишется всё дерево
    /// зависимостей пакетов</param>
    /// <param name="npmCmd">Путь к файлу npm.cmd из nodejs</param>
    /// <param name="workingDirectory">Путь к корневой папке веб-проекта, там
    /// где package.json и node_modules</param>
    public static async Task NpmGetPackageList(string outputFileName, string npmCmd,
        string? workingDirectory)
    {
        using (StreamWriter writer = new StreamWriter(outputFileName))
        {
            await writer.WriteLineAsync(string.Empty);
        }

        var processInfo = new ProcessStartInfo(npmCmd, $"ls --all >{outputFileName}")
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory
        };

        var process = Process.Start(processInfo);
        process?.WaitForExit();
        process?.Close();
    }

    /// <summary>
    /// Получение даты релиза версии
    /// </summary>
    /// <param name="packageName">Название пакета</param>
    /// <param name="packageVersion">Версия пакета</param>
    /// <param name="npmCmd">Путь к файлу npm.cmd из nodejs</param>
    /// <returns></returns>
    private static string NpmGetPackageDate(string packageName, string packageVersion,
        string npmCmd)
    {
        //Имя временного файла, в который пишется информация о всех существующих версиях пакета.
        var infoFileName = $"{Path.Combine(Directory.GetCurrentDirectory(), "info.json")}";
        var processInfo = new ProcessStartInfo(npmCmd,
            $"view {packageName} time --json >{infoFileName}")
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true
        };

        var process = Process.Start(processInfo);
        process.WaitForExit();
        process.Close();

        //Выбираем строку с нужной версией и вычленяем оттуда дату.
        var datePattern = @"\d\d\d\d-\d\d-\d\d";
        var lines = File.ReadLines(infoFileName).Skip(1);
        foreach (string line in lines)
        {
            if (line.Contains(packageVersion))
            {
                var matches = Regex.Matches(line, datePattern);
                return matches[0].Value;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Создает словарь с наименованием пакета и версиями из текстовых данных файла
    /// </summary>
    /// <param name="fileName">Файл из которого производится чтение</param>
    /// <returns></returns>
    public static Dictionary<string, List<string>> NpmParseFile(string fileName)
    {
        var result = new Dictionary<string, List<string>>();
        var lines = File.ReadLines(fileName).Skip(1);
        foreach (string line in lines)
        {
            //выкидываем строки без пакетов и необязательные пакеты.
            if (string.IsNullOrWhiteSpace(line) || line.Contains("__ngcc_entry_points")
                || line.Contains("UNMET OPTIONAL DEPENDENCY"))
            {
                continue;
            }

            //выкидываем лишние слова из строк.
            var formattedLine = line.Replace(" deduped", string.Empty)
                .Replace(" extraneous", string.Empty);

            //вычленяем наименование пакета и версию.
            var delimeterIndex = formattedLine.LastIndexOf("@");
            var startPackageNameIndex = formattedLine.LastIndexOf(" ") + 1;
            var packageName = formattedLine.Substring(startPackageNameIndex,
                delimeterIndex - startPackageNameIndex);
            var version = formattedLine.Substring(delimeterIndex + 1,
                formattedLine.Length - delimeterIndex - 1);

            if (version.Contains("||"))
            {
                version = version.Substring(0, version.IndexOf(" "));
            }

            version = version.Replace("^", string.Empty).Replace("~", string.Empty);
            if (!result.ContainsKey(packageName))
            {
                result.Add(packageName, new List<string> {version});
            }
            else
            {
                if (!result[packageName].Contains(version))
                {
                    result[packageName].Add(version);
                }
            }
        }

        return result;
    }
}