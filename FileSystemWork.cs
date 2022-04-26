using System.Collections.Specialized;

namespace GetVersionNugetPackages;

/// <summary>
/// Класс для роботы с файловой системой
/// </summary>
public class FileSystemWork
{
    /// <summary>
    /// Рекурсивный метод для поиска файлов с определенным расширением
    /// в указанной папке и вывода названий найденных файлов в консоль.
    /// </summary>
    /// <param name="root">Экземпляр класса DirectoryInfo с переданным
    /// адресом папки в которой необходимо произвести поиск файлов</param>
    /// <param name="log">Коллекция строк для занесения возможных ошибок</param>
    /// <param name="projects">Коллекция FileInfo с информацией о найденных
    /// файлах</param>
    /// <param name="fileExtension"></param>
    public static void WalkFile(DirectoryInfo root, ref StringCollection log,
        ref List<FileInfo> projects, string fileExtension)
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
            //выводим имена файлов в консоль
            foreach (FileInfo fi in files)
            {
                if (fi.Name.EndsWith(fileExtension))
                {
                    projects.Add(fi);
                    Console.WriteLine(fi.Name);
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
}