using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SlippiNET.Analysers;
using SlippiNET.Models.Commands;
using SlippiNET.Utils;

namespace TestConsoleApp
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
			var files = Directory.GetFiles(@"C:\Users\bartd\Documents\Slippi", "*.slp");
			//var files = new[]
			//{
			//    @"C:\Users\bartd\Documents\Slippi\Game_20210503T185529.slp"
			//};
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			int hitLCancels = default;
			int missedLCancels = default;
			var self = "BORT#186";

			var records = new Dictionary<string, (int Won, int Lost)>();
			foreach (var file in files)
			{
				try
				{
					// Read all the commands from the file.
					var game = new SlippiGame(file, self);
					game.Analyse();
					var analysisInput = game.AnalysisInput;
					if (!records.ContainsKey(analysisInput.OpponentCode))
					{
						records.Add(analysisInput.OpponentCode, (0, 0));
					}

					if (game.Won(self))
					{
						var record = records[analysisInput.OpponentCode];
						record.Won++;
						records[analysisInput.OpponentCode] = record;
						Console.WriteLine("{0} won against {1}", self, analysisInput.OpponentCode);
					}
					else
					{
						var record = records[analysisInput.OpponentCode];
						record.Lost++;
						records[analysisInput.OpponentCode] = record;
						Console.WriteLine("{0} lost against {1}", self, analysisInput.OpponentCode);
					}


				}
				catch (Exception exception)
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
