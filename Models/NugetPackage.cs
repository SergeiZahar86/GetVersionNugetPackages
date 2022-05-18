using System;

namespace GetVersionNugetPackages.Models;

/// <summary>
/// Модель предоставляющая информацию о Nuget пакете
/// </summary>
public class NugetPackage
{
    /// <summary>
    /// Название пакета
    /// </summary>
    private string Name { get; set; }
    
    /// <summary>
    /// Версия
    /// </summary>
    private string Version { get; set; }
    
    /// <summary>
    /// Дата релиза
    /// </summary>
    private DateTime Release { get; set; }
}