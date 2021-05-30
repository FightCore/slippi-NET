using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
					// Open the file read
					var binaryFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					// Get the file type that includes all the message sizes and data locations.
					var fileType = new SlippiTypeFileReader().GetFileType(binaryFile);

					if (fileType.MetadataPosition == -1)
					{
						Console.WriteLine("No meta data available");
						continue;
					}
					var metaData = new SlippiMetaDataReader().Read(binaryFile, fileType);

					var selfIndex = metaData.Players[0].Names["code"] == self ? 0 : 1;
					var opponentIndex = selfIndex == 0 ? 1 : 0;

					// Read all the commands from the file.
					var commands = new SlippiFileReader().Read(binaryFile, fileType);
					var commandsAsList = commands.ToList();
					var selfLastFrame =
						commandsAsList.Last(command => command is SlippiPostFrameUpdateCommand postFrameUpdateCommand && postFrameUpdateCommand.PlayerIndex == selfIndex) as
							SlippiPostFrameUpdateCommand;
					var opponentLastFrame =
						commandsAsList.Last(command => command is SlippiPostFrameUpdateCommand postFrameUpdateCommand && postFrameUpdateCommand.PlayerIndex == opponentIndex) as
							SlippiPostFrameUpdateCommand;

					var won = selfLastFrame.StocksRemaining > opponentLastFrame.StocksRemaining;


					if (selfLastFrame.StocksRemaining == opponentLastFrame.StocksRemaining)
					{
						if (selfLastFrame.StocksRemaining != 1 || opponentLastFrame.StocksRemaining != 1)
						{
							Console.WriteLine("Game didn't end on one stock, most likely laggy quit.");
						}

						var gameEndCommand =
							commandsAsList.First(command => command is SlippiGameEndCommand) as SlippiGameEndCommand;
						if (gameEndCommand.LRASInitiator == -1)
						{
							Console.WriteLine("Game ended equal without LRAS indication, doesn't count.");
						}
						won = gameEndCommand.LRASInitiator != selfIndex;
					}


					var opponentCode = metaData.Players[opponentIndex].Names["code"];

					if (!records.ContainsKey(opponentCode))
					{
						records.Add(opponentCode, (0, 0));
					}

					if (won)
					{
						var record = records[opponentCode];
						record.Won++;
						records[opponentCode] = record;
						Console.WriteLine("Playing {0} Won against {1} as {2}", metaData.Players[selfIndex].Characters.First().Key, metaData.Players[opponentIndex].Names["code"], metaData.Players[opponentIndex].Characters.First().Key);
					}
					else
					{
						var record = records[opponentCode];
						record.Lost++;
						records[opponentCode] = record;
						Console.WriteLine("Playing {0} Lost against {1} as {2}", metaData.Players[selfIndex].Characters.First().Key, metaData.Players[opponentIndex].Names["code"], metaData.Players[opponentIndex].Characters.First().Key);
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
