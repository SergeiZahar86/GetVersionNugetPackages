namespace GetVersionNugetPackages.Models;

/// <summary>
/// Модель предоставляющая информацию об иследуемом проекте
/// </summary>
public class ProjectInformation
{
    /// <summary>
    /// Название проекта
    /// </summary>
    private string Name { get; set; }
    
    /// <summary>
    /// Путь к проекту
    /// </summary>
    private string Path { get; set; }
    
    /// <summary>
    /// Список Nuget пакетов
    /// </summary>
    private IEnumerable<NugetPackage> NugetPackages { get; set; }
    
    /// <summary>
    /// Список Npm пакетов
    /// </summary>
    private IEnumerable<NpmPackage> NpmPackages { get; set; }
}