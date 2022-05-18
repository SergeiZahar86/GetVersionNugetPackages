using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using GetVersionNugetPackages.CommonLogic;
using GetVersionNugetPackages.Models;

namespace GetVersionNugetPackages;

public class Program
{
    /// <summary>
    /// Адрес репозитория
    /// </summary>
    private const string SourceRepository = "https://api.nuget.org/v3/index.json";

    /// <summary>
    /// Адрес исследуемой директории
    /// </summary>
    static string RootDir =
        @"D:\Projec_Visual_Studio\2022\My_old_microservises\PJ1.Frontend";

    /// <summary>
    /// Искомое расширение файлов
    /// </summary>
    private const string FileExtension = ".csproj";

    /// <summary>
    /// Дата старше которой не должны быть Nuget и Npm пакеты
    /// </summary>
    private static readonly DateTime CriticalDate = new DateTime(2022, 2, 23);

    /// <summary>
    /// Путь к файлу npm.cmd из nodejs
    /// </summary>
    private const string NpmCmd = @"C:\Program Files\nodejs16\npm.cmd";

    /// <summary>
    /// Путь к корневой папке веб-проекта, там где package.json и node_modules
    /// </summary>
    private const string WorkingDirectory =
        @"D:\Projec_Visual_Studio\2022\My_old_microservises\PJ1.Frontend\PJ1.Frontend\PJ1.FrontByAngular\ClientApp";

    /// <summary>
    /// Имя результирующего файла для пакетов с единственной версией, в который
    /// пишется наименование, версия и дата версии пакета.
    /// </summary>
    private const string ProjectsMetadataFile = "Projects_Metadata.txt";

    /// <summary>
    /// Имя временного файла, в который пишется всё дерево зависимостей пакетов.
    /// </summary>
    private const string LogFile = "log.txt";

    static async Task Main(string[] args)
    {
        if (args.Length != 0)
        {
            RootDir = args[0];
        }

        var logFile = $"{Path.Combine(Directory.GetCurrentDirectory(), LogFile)}";

        var resulFileName =
            $"{Path.Combine(Directory.GetCurrentDirectory(), ProjectsMetadataFile)}";

        StringCollection log = new StringCollection();
        List<ProjectInformation> projects = new List<ProjectInformation>();

        CommonLogic.FileSystemWork.GetProjectFiles(new DirectoryInfo(RootDir), ref log, ref projects,
            FileExtension);

        CommonLogic.FileSystemWork.GetFilesPackageJson(ref projects, ref log);

        await NugetPackageMetadata.NugetPackageGetAndWriteMetadataToFile(projects, SourceRepository,
            resulFileName, CriticalDate);

        await NpmPackageMetadata.NpmGetAndWriteMetadataToFile(projects, logFile, NpmCmd,
            resulFileName, CriticalDate);

        //DataDisplay.WorkingWithConsole(projects, CriticalDate);

        Console.WriteLine("End");
        Console.Read();
    }
}