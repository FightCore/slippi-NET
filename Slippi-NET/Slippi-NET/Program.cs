using SlippiReader.Utils;
using System;
using System.IO;
using System.Text;

namespace SlippiReader
{
    class Program
    {
        static void Main(string[] args)
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
                @"C:\Users\bartd\Documents\Slippi\Game_20210420T204501.slp"
            };
            foreach (var file in files)
            {
                try
                {
                    var binaryFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var fileType = SlippiTypeFileReader.GetFileType(binaryFile);
                    new SlippiFileReader().Read(binaryFile, fileType);
                }
                catch
                {

                }

            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath);
            var binaryFile = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var fileType = SlippiTypeFileReader.GetFileType(binaryFile);
            new SlippiFileReader().Read(binaryFile, fileType);
        }
    }
}
