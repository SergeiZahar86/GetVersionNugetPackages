using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using GetVersionNugetPackages.Models;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace GetVersionNugetPackages.CommonLogic;

/// <summary>
/// Класс для определения метаинформации о Nuget пакетах
/// </summary>
public class NugetPackageMetadata
{
    /// <summary>
    /// Получает даты релиза искомых nuget пакетов 
    /// </summary>
    /// <param name="sourceRepository">Адрес репозитория</param>
    /// <param name="projectInformations">Коллекция проектов</param>
    /// <returns>Коллекция проектов с датами релизов</returns>
    public static async Task<List<ProjectInformation>> GetNugetPackagesReleaseDate(
        string sourceRepository,
        List<ProjectInformation> projectInformations)
    {
        ILogger logger = NullLogger.Instance;
        CancellationToken cancellationToken = CancellationToken.None;
        SourceCacheContext cache = new SourceCacheContext();
        SourceRepository repository = Repository.Factory.GetCoreV3(sourceRepository);
        PackageMetadataResource resource =
            await repository.GetResourceAsync<PackageMetadataResource>();

        foreach (var projectInformation in projectInformations)
        {
            foreach (var nugetPackage in projectInformation.NugetPackages)
            {
                IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
                    nugetPackage.Name, // заменить с требуемым идентификатором пакета
                    includePrerelease: true,
                    includeUnlisted: false,
                    cache,
                    logger,
                    cancellationToken);

                foreach (IPackageSearchMetadata package in packages)
                {
                    if (nugetPackage.Version == package.Identity.Version.ToString())
                    {
                        nugetPackage.Release = (((DateTimeOffset) package.Published)!).DateTime;
                    }
                }
            }
        }

        return projectInformations;
    }

    /// <summary>
    /// Определение названий и версий Nuget пакетов
    /// </summary>
    /// <param name="projects">Коллекция ProjectFileInfo с информацией о найденных
    /// проектах</param>
    private static List<ProjectInformation> GetNugetVersionPackage(
        List<ProjectInformation> projects)
    {
        XmlDocument xDoc = new XmlDocument();
        foreach (var project in projects)
        {
            xDoc.Load(project.ProjectFileInfo.FullName);

            // получим корневой элемент
            XmlElement? xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе
                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name == "ItemGroup")
                    {
                        foreach (XmlNode childNode in xnode.ChildNodes)
                        {
                            if (childNode.Name == "PackageReference")
                            {
                                if (childNode.Attributes?.Count == 2
                                    && childNode.Attributes[1].Name == "Version")
                                {
                                    NugetPackage nugetPackage = new NugetPackage();
                                    nugetPackage.Name = childNode.Attributes[0].Value;
                                    nugetPackage.Version = childNode.Attributes[1].Value;
                                    project.NugetPackages.Add(nugetPackage);
                                }
                            }
                        }
                    }
                }
            }
        }

        return projects;
    }


    public static async Task NugetPackageGetAndWriteMetadataToFile(
        List<ProjectInformation> projects,
        string sourceRepository, string resulFileName, DateTime criticalDate)
    {
        projects = GetNugetVersionPackage(projects);
        projects = await GetNugetPackagesReleaseDate(sourceRepository, projects);

        using (StreamWriter writer = new StreamWriter(resulFileName))
        {
            writer.WriteLine("-----------------------------------");
            writer.WriteLine("------- Nuget Packages ------------");
            writer.WriteLine("-----------------------------------");
            writer.WriteLine(string.Empty);
            foreach (var project in projects)
            {
                writer.WriteLine($"-->  {project.ProjectFileInfo.Name}");
                writer.WriteLine(string.Empty);
                foreach (var nugetPackage in project.NugetPackages)
                {
                    if (criticalDate != null && nugetPackage.Release >= criticalDate)
                    {
                        writer.WriteLine($"*  {nugetPackage.Name} {nugetPackage.Version}"
                            + $" {nugetPackage.Release}");
                    }
                    else
                    {
                        writer.WriteLine($"{nugetPackage.Name} {nugetPackage.Version}"
                            + $" {nugetPackage.Release}");
                    }
                }
            }
        }
    }
}