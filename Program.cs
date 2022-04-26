using System.Collections.Specialized;
using System.Xml;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;



// ----  Работа с файловой системой -------------------------------------------
StringCollection log = new StringCollection();
List<FileInfo> projects = new List<FileInfo>();

//задаем папку для обхода
string rootDir = @"D:/Projec_Visual_Studio/2022/My_old_microservises/"
    + "WorkTime.AuthService.WebApi-master/WorkTime.AuthService.WebApi-master";

// рекурсивный метод
void WalkFile(DirectoryInfo root)
{
    FileInfo[] files = null;
    DirectoryInfo[] subDirs = null;
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
            if (fi.Name.EndsWith(".csproj"))
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
            WalkFile(dirInfo);
        }
    }
}

//вызываем рекурсивный метод
WalkFile(new DirectoryInfo(rootDir));

Console.WriteLine("Файлы, доступ к которым запрещен:");
foreach (string s in log)
{
    Console.WriteLine(s);
}

//**** Работа с XML файла проекта  **********************************************************************
Dictionary<string, string> namesPackages = new Dictionary<string, string>();
XmlDocument xDoc = new XmlDocument();
foreach (var project in projects)
{
    xDoc.Load(project.FullName);
    // получим корневой элемент
    XmlElement? xRoot = xDoc.DocumentElement;
    if (xRoot != null)
    {
        // обход всех узлов в корневом элементе
        foreach (XmlElement xnode in xRoot)
        {
            if (xnode.Name == "ItemGroup")
            {
                Console.WriteLine(xnode.Name);
                foreach (XmlNode childNode in xnode.ChildNodes)
                {
                    Console.WriteLine(childNode.Name);
                    if (childNode.Name == "PackageReference")
                    {
                        if (childNode.Attributes?.Count == 2 && childNode.Attributes[1].Name == "Version")
                        {
                            Console.WriteLine("dfdfdfdfdfdfdfd");
                            
                            
                            foreach (XmlAttribute attribute in childNode.Attributes)
                            {
                                //Console.WriteLine(attribute.Name);
                                if (attribute.Name == "Include")
                                {
                                    
                                }
                            }
                        }
                    }
                }
            }

            // получаем атрибут name
            // XmlNode? attr = xnode.Attributes.GetNamedItem("name");
            // Console.WriteLine(attr?.Value);
            //
            // // обходим все дочерние узлы элемента user
            // foreach (XmlNode childnode in xnode.ChildNodes)
            // {
            //     // если узел - company
            //     if (childnode.Name == "company")
            //     {
            //         Console.WriteLine($"Company: {childnode.InnerText}");
            //     }
            //
            //     // если узел age
            //     if (childnode.Name == "age")
            //     {
            //         Console.WriteLine($"Age: {childnode.InnerText}");
            //     }
            // }
        }
    }

    Console.WriteLine();
}

Console.Read();



//********  Получение через Api информации о Nuget пакете  ******************************************************************
//Используйте пакет SDK NuGet . Я изменил код для отображения свойства Published.

ILogger logger = NullLogger.Instance;
CancellationToken cancellationToken = CancellationToken.None;

SourceCacheContext cache = new SourceCacheContext();
SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();

IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
    "IdentityModel", // заменить с требуемым идентификатором пакета
    includePrerelease: true,
    includeUnlisted: false,
    cache,
    logger,
    cancellationToken);

foreach (IPackageSearchMetadata package in packages)
{
    Console.WriteLine($"Version: {package.Identity.Version}");
    Console.WriteLine($"Publish date: {package.Published}");
}

Console.Read();


// 1111
/*
if (Directory.Exists("C:/Users/sergei/.nuget/packages"))
{
    Console.WriteLine("Directory.Exists");
    var dirs = Directory.GetDirectories("C:/Users/sergei/.nuget/packages");
    foreach (var dir in dirs)
    {
        string[] words = dir.Split(new char[] { '/', '\\' });
        Console.WriteLine(words.Last() + "   **********************************");
        var dirsVersions = Directory.GetDirectories(dir);
        foreach (var dirVersion in dirsVersions)
        {
            string[] dirV = dirVersion.Split(new char[] { '/', '\\' });
            Console.WriteLine("    " + dirV.Last() + "  <<<");
            var files = Directory.GetFiles(dirVersion);
            foreach (var file in files)
            {
                if (file.EndsWith(".nuspec"))
                {
                    string[] partFile = file.Split(new char[] { '/', '\\' });
                    Console.WriteLine("        " + partFile.Last());
                    Console.WriteLine("        " + Directory.GetLastWriteTime(file).ToShortDateString());
                }
            }
        }
        Console.WriteLine();
        Console.WriteLine();
    }

    Console.Read();
}
*/


///2222


//// 3333
/*
if (Directory.Exists("D:/Projec_Visual_Studio/2022/My_old_microservises/"
    + "WorkTime.AuthService.WebApi-master/WorkTime.AuthService.WebApi-master"))
{
    Console.WriteLine("Directory Exists");
    List<string> projects = new List<string>();

    var files = Directory.GetFiles("D:/Projec_Visual_Studio/2022/"
        + "My_old_microservises/WorkTime.AuthService.WebApi-master/"
        + "WorkTime.AuthService.WebApi-master");

    foreach (var file in files)
    {
        if (file.EndsWith(".csproj"))
        {
            projects.Add(file);
        }
    }
    
    
    var dirs = Directory.GetDirectories("D:/Projec_Visual_Studio/2022/"
        + "My_old_microservises/WorkTime.AuthService.WebApi-master/"
        + "WorkTime.AuthService.WebApi-master");
    
    
    
    
    
    
    
    
    foreach (var dir in dirs)
    {
        string[] words = dir.Split(new char[] { '/', '\\' });
        Console.WriteLine(words.Last() + "   **********************************");
        var dirsVersions = Directory.GetDirectories(dir);
        foreach (var dirVersion in dirsVersions)
        {
            string[] dirV = dirVersion.Split(new char[] { '/', '\\' });
            Console.WriteLine("    " + dirV.Last() + "  <<<");
            files = Directory.GetFiles(dirVersion);
            foreach (var file in files)
            {
                if (file.EndsWith(".nuspec"))
                {
                    string[] partFile = file.Split(new char[] { '/', '\\' });
                    Console.WriteLine("        " + partFile.Last());
                    Console.WriteLine("        " + Directory.GetLastWriteTime(file).ToShortDateString());
                }
            }
        }
        Console.WriteLine();
        Console.WriteLine();
    }
}
else
{
    Console.WriteLine("Directory not Exists");
}
*/