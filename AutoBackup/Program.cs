using Serilog;
using System.IO;
using Newtonsoft.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("C:\\Users\\stuar\\OneDrive\\Desktop\\Projects\\AutoBackup\\Log.txt")
            .CreateLogger();

        string json = System.IO.File.ReadAllText("AppSettings.json");
      
        Directories[] Dirs = JsonConvert.DeserializeObject<Directories[]>(json);

        int count = 0;
        string destination = "C:\\Users\\stuar\\OneDrive\\Desktop\\Laptop-Backup";


        foreach (Directories d in Dirs)
        {
           
            try
            {
                string[] GetFiles = Directory.GetFiles(d.dir, "*", System.IO.SearchOption.AllDirectories);
                Console.WriteLine($"Getting Files from Directory:{d.dir} ");
                
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
                        count++;
                    }
                    else if (Directory.Exists(destination) && File.Exists(target))
                    {
                        FileInfo fileInfoSource = new FileInfo(file);
                        FileInfo fileInfoTarget = new FileInfo(target);

                        if (fileInfoSource.LastWriteTime > fileInfoTarget.LastWriteTime)
                        {
                            File.Copy(file, target, true);
                            Console.WriteLine($"{file} was overwritten");
                            count++;
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
                        count++;
                    }
                }
                
            }
            catch (IOException)
            {

               Log.Error($"Could not access directory:{d.dir}");

            }
            
        }
        Console.WriteLine("Number of files backed up: " + count);
        Log.Information("Number of files backed up: " + count);
        Log.CloseAndFlush();    
    }




       
}