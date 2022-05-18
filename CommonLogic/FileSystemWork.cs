using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using GetVersionNugetPackages.Models;

namespace GetVersionNugetPackages.CommonLogic;

/// <summary>
/// Класс для роботы с файловой системой
/// </summary>
public class FileSystemWork
{
    /// <summary>
    /// Рекурсивный метод для поиска файлов с определенным расширением
    /// в указанной папке
    /// </summary>
    /// <param name="root">Экземпляр класса DirectoryInfo с переданным
    /// адресом папки в которой необходимо произвести поиск файлов</param>
    /// <param name="log">Коллекция строк для занесения возможных ошибок</param>
    /// <param name="projects">Коллекция ProjectFileInfo с информацией о найденных
    /// файлах</param>
    /// <param name="fileExtension">Искомое расширение файлов</param>
    private static void WalkFile(DirectoryInfo root, ref StringCollection log,
        ref List<ProjectInformation> projects, string fileExtension)
    {
        FileInfo[]? files = null;
        DirectoryInfo[]? subDirs = null;
        // Получаем все файлы в текущем каталоге
        try
        {
            files = root.GetFiles("*.*");
        }
        catch (UnauthorizedAccessException e)
        {
            log.Add(e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (files != null)
        {
            foreach (FileInfo fi in files)
            {
                if (fi.Name.EndsWith(fileExtension))
                {
                    ProjectInformation project = new ProjectInformation();
                    project.ProjectFileInfo = fi;
                    projects.Add(project);
                }
            }

            //получаем все подкаталоги
            subDirs = root.GetDirectories();
            //проходим по каждому подкаталогу
            foreach (DirectoryInfo dirInfo in subDirs)
            {
                //РЕКУРСИЯ
                WalkFile(dirInfo, ref log, ref projects, fileExtension);
            }
        }
    }

    /// <summary>
    /// Метод для поиска файлов с определенным расширением
    /// в указанной папке
    /// </summary>
    /// <param name="root">Экземпляр класса DirectoryInfo с переданным
    /// адресом папки в которой необходимо произвести поиск файлов</param>
    /// <param name="log">Коллекция строк для занесения возможных ошибок</param>
    /// <param name="projects">Коллекция ProjectFileInfo с информацией о найденных
    /// файлах</param>
    /// <param name="fileExtension">Искомое расширение файлов</param>
    public static void GetProjectFiles(DirectoryInfo root, ref StringCollection log,
        ref List<ProjectInformation> projects, string fileExtension)
    {
        WalkFile(root, ref log, ref projects, fileExtension);
        foreach (var project in projects)
        {
            //project.NpmPackages = new List<NpmPackage>();
            project.NugetPackages = new List<NugetPackage>();
        }

        if (log.Count > 0)
        {
            Console.WriteLine("Файлы, доступ к которым запрещен:");
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Получение информации о возможно имеющемся в проекте файле package.json
    /// </summary>
    /// <param name="projects">Коллекция ProjectFileInfo с информацией о проектах</param>
    public static void GetFilesPackageJson(ref List<ProjectInformation> projects,
        ref StringCollection log)
    {
        foreach (ProjectInformation project in projects)
        {
            DirectoryInfo? parent = Directory.GetParent(project.ProjectFileInfo.FullName);
            SearchFile(parent, ref log, project );

        }
        if (log.Count > 0)
        {
            Console.WriteLine("Файлы, доступ к которым запрещен:");
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
        }
    }

    /// <summary>
    /// Рекурсивный метод для поиска package.json
    /// </summary>
    /// <param name="root">Экземпляр класса DirectoryInfo с переданным
    /// адресом папки в которой необходимо произвести поиск файлов</param>
    /// <param name="log">Коллекция строк для занесения возможных ошибок</param>
    /// <param name="project">ProjectInformation с информацией о C# проекте</param>
    private static void SearchFile(DirectoryInfo root, ref StringCollection log,
        ProjectInformation project)
    {
        FileInfo[]? files = null;
        DirectoryInfo[]? subDirs = null;
        // Получаем все файлы в текущем каталоге
        try
        {
            files = root.GetFiles("*.*");
        }
        catch (UnauthorizedAccessException e)
        {
            log.Add(e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (files != null)
        {
            foreach (FileInfo fi in files)
            {
                if (fi.Name == "package.json" && project.PackageJsonFileInfo == null)
                {
                    project.PackageJsonFileInfo = fi;
                }
            }

            //получаем все подкаталоги
            subDirs = root.GetDirectories();
            //проходим по каждому подкаталогу
            foreach (DirectoryInfo dirInfo in subDirs)
            {
                //РЕКУРСИЯ
                SearchFile(dirInfo, ref log, project);
            }
        }
    }
}