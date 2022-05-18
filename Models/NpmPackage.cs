using System;

namespace GetVersionNugetPackages.Models;

/// <summary>
/// Модель предоставляющая информацию о Npm пакете
/// </summary>
public class NpmPackage
{
    /// <summary>
    /// Название пакета
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Версия
    /// </summary>
    public string Version { get; set; }
    
    /// <summary>
    /// Дата релиза
    /// </summary>
    public DateTime Release { get; set; }
}