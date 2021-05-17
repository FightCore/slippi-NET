using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SlippiNET.Models.Commands;
using SlippiNET.Utils;

namespace SlippiNET
{
    class Program
    {
        static void Main(string[] _)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //using var watcher = new FileSystemWatcher(@"C:\Users\bartd\Documents\Slippi");
            //watcher.NotifyFilter = NotifyFilters.FileName;
            //watcher.Created += Watcher_Created;
            //watcher.Filter = "*.slp";
            //watcher.IncludeSubdirectories = true;
            //watcher.EnableRaisingEvents = true;
            //var files = Directory.GetFiles(@"C:\Users\bartd\Documents\Slippi", "*.slp");
            var files = new[]
            {
                @"C:\Users\bartd\Documents\Slippi\Game_20210502T143311.slp"
            };
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int hitLCancels = default;
            int missedLCancels = default;
            foreach (var file in files)
            {
                try
                {
                    // Open the file read
                    var binaryFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    // Get the file type that includes all the message sizes and data locations.
                    var fileType = new SlippiTypeFileReader().GetFileType(binaryFile);
                    // Read all the commands from the file.
                    var commands = new SlippiFileReader().Read(binaryFile, fileType);
                    // ToList to avoid multiple iterations meaning incorrect data
                    var listCommands = commands.ToList();
                    // Calculate hit
                    hitLCancels = listCommands.Count(command =>
                        command is SlippiPostFrameUpdateCommand {LCancelStatus: 1, PlayerIndex: 0});

                    // Calculate missed.
                    missedLCancels = listCommands.Count(command =>
                        command is SlippiPostFrameUpdateCommand {LCancelStatus: 2, PlayerIndex: 0});
                }
                catch(Exception exception)
                {
                    Console.WriteLine("{0} failed to read", file);
                    Console.WriteLine(exception);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Calculated in {0} ms", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("Hit LCancels : {0}", hitLCancels);
            Console.WriteLine("Missed LCancels : {0}", missedLCancels);

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath);
            var binaryFile = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var fileType = new SlippiTypeFileReader().GetFileType(binaryFile);
            new SlippiFileReader().Read(binaryFile, fileType);
        }
    }
}
