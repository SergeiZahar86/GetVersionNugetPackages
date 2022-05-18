using System.Collections.Generic;
using System.IO;

namespace GetVersionNugetPackages.Models;

/// <summary>
/// Модель предоставляющая информацию об иследуемом проекте
/// </summary>
public class ProjectInformation
{
    /// <summary>
    /// Информация о файле проекта
    /// </summary>
    public FileInfo ProjectFileInfo { get; set; }
    
    /// <summary>
    /// Список Nuget пакетов
    /// </summary>
    public List<NugetPackage> NugetPackages { get; set; }
    
    // /// <summary>
    // /// Список Npm пакетов
    // /// </summary>
    // public List<NpmPackage> NpmPackages { get; set; }
    
    /// <summary>
    /// Информация о возможно имеющемся в проекте файле package.json
    /// </summary>
    public FileInfo PackageJsonFileInfo { get; set; }
}