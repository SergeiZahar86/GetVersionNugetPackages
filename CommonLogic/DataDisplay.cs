using System;
using System.Collections.Generic;
using GetVersionNugetPackages.Models;

namespace GetVersionNugetPackages.CommonLogic;

/// <summary>
/// Класс для отображения данных
/// </summary>
public class DataDisplay
{
    /// <summary>
    /// Выводит данные в консоль
    /// </summary>
    /// <param name="projects">Коллекция проектов</param>
    /// <param name="criticalDate">Дата старше которой не должны быть Nuget и Npm пакеты</param>
    public static void WorkingWithConsole(List<ProjectInformation> projects,
        DateTime criticalDate)
    {
        foreach (var project in projects)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {project.ProjectFileInfo.Name}  <<<");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  Own Nuget Packages");
            Console.ResetColor();
            Console.WriteLine();
            
            if (project.NugetPackages.Count > 0)
            {
                foreach (var pack in project.NugetPackages)
                {
                    if (pack.Release > criticalDate)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"! >    {pack.Name}");
                        Console.WriteLine($"      {pack.Version}  --   "
                            + $"{pack.Release.ToShortDateString()}");
                        Console.ResetColor(); // сбрасываем в стандартный
                    }

                    Console.WriteLine($"  {pack.Name}");
                    Console.WriteLine($"      {pack.Version}  --   "
                        + $"{pack.Release.ToShortDateString()}");
                }
            }
            else
            {
                Console.WriteLine("  ---  Nothing found");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  Own NPM Packages");
            Console.ResetColor();
            
            // if (project.NpmPackages.Count > 0)
            // {
            //     foreach (var pack in project.NpmPackages)
            //     {
            //         if (pack.Release > criticalDate)
            //         {
            //             Console.ForegroundColor = ConsoleColor.Red;
            //             Console.WriteLine($"! >    {pack.Name}");
            //             Console.WriteLine($"      {pack.Version}  --   "
            //                 + $"{pack.Release.ToShortDateString()}");
            //             Console.ResetColor(); // сбрасываем в стандартный
            //         }
            //
            //         Console.WriteLine($"  {pack.Name}");
            //         Console.WriteLine($"      {pack.Version}  --   "
            //             + $"{pack.Release.ToShortDateString()}");
            //     }
            // }
            // else
            // {
            //     Console.WriteLine("  ---  Nothing found");
            // }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}