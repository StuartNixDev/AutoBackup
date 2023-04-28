using Serilog;
using System.IO;
using Newtonsoft.Json;

internal partial class Program
{

    private static void Main(string[] args)
    {
        InitiateLog();
        Directories[] Dirs = GetDirectories();
        string destination = "C:\\Users\\stuar\\OneDrive\\Desktop\\Laptop-Backup";
        RunBackup(Dirs, destination);
        Console.WriteLine("Number of files backed up: " + Counter.count);
        Log.Information("Number of files backed up: " + Counter.count);
        Log.CloseAndFlush();
    }

    private static void RunBackup(Directories[] Dirs, string destination)
    {
        foreach (Directories d in Dirs)
        {

            try
            {
                string[] GetFiles = FetchFilesFromDirectory(d);

                foreach (string file in GetFiles)

                {
                    string fileName = Path.GetFileName(file);
                    string target = Path.Combine(destination, fileName);


                    if (!Directory.Exists(destination))
                    {
                        Directory.CreateDirectory(destination);
                        Console.WriteLine($"Directory {destination} has been created");
                        File.Copy(file, target, true);
                        Console.WriteLine(file);
                        Counter.count++;
                    }
                    else if (Directory.Exists(destination) && File.Exists(target))
                    {
                        FileInfo fileInfoSource = new FileInfo(file);
                        FileInfo fileInfoTarget = new FileInfo(target);

                        if (fileInfoSource.LastWriteTime > fileInfoTarget.LastWriteTime)
                        {
                            File.Copy(file, target, true);
                            Console.WriteLine($"{file} was overwritten");
                            Counter.count++;
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else
                    {
                        File.Copy(file, target, true);
                        Console.WriteLine(file);
                        Counter.count++;
                    }
                }

            }
            catch (IOException)
            {

                Log.Error($"Could not access directory:{d.dir}");

            }

        }
    }

    private static string[] FetchFilesFromDirectory(Directories d)
    {
        string[] GetFiles = Directory.GetFiles(d.dir, "*", System.IO.SearchOption.AllDirectories);
        Console.WriteLine($"Getting Files from Directory:{d.dir} ");
        return GetFiles;
    }

    private static Directories[] GetDirectories()
    {
        string json = System.IO.File.ReadAllText("AppSettings.json");
        Directories[] Dirs = JsonConvert.DeserializeObject<Directories[]>(json);
        return Dirs;
    }

    private static void InitiateLog()
    {
        Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("C:\\Users\\stuar\\OneDrive\\Desktop\\Projects\\AutoBackup\\Log.txt")
                    .CreateLogger();
    }

}