using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SlippiNET.Analyzers.Analyzers;
using SlippiNET.Analyzers.Analyzers.Performance;
using SlippiNET.Analyzers.DTOs;
using SlippiNET.Analyzers.DTOs.Performance;
using SlippiNET.Models.Commands;
using SlippiNET.Utils;

namespace SlippiNET.Analyzers
{
	public class SlippiGame
	{
		private readonly string _filePath;
		private List<BaseSlippiCommand> _commandList;

		public GameResultDto GameResult { get; private set; }

		public AnalysisInputDto AnalysisInput { get; set; }

		public PerformanceDto Performance { get; set; }

		public SlippiGame(string filePath, string playerCode)
		{
			_filePath = filePath;
			AnalysisInput = new AnalysisInputDto()
			{
				PlayerCode = playerCode
			};
		}

		public void Analyze()
		{
			// Open the file read
			var binaryFile = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			// Get the file type that includes all the message sizes and data locations.
			var fileType = new SlippiTypeFileReader().GetFileType(binaryFile);

			if (fileType.MetadataPosition == -1)
			{
				throw new Exception("No meta data available");
			}
			var metaData = new SlippiMetaDataReader().Read(binaryFile, fileType);
			
			// Check which index the player is playing under by going over the players names and searching for "code" 
			AnalysisInput.PlayerIndex = metaData.Players[0].Names["code"] == AnalysisInput.PlayerCode ? 0 : 1;
			// Set the opponent index as the opposite of the player index.
			// TODO Improve this for other port usage and doubles
			AnalysisInput.OpponentIndex = AnalysisInput.PlayerIndex == 0 ? 1 : 0;
			// Set the code of the opponent using the previously found index.
			AnalysisInput.OpponentCode = metaData.Players[AnalysisInput.OpponentIndex].Names["code"];

			// Get the commands enumerable from the slippi file.
			var commands = new SlippiFileReader().Read(binaryFile, fileType);

			// Convert it to a list as we will need to iterate the file multiple times.
			_commandList = commands.ToList();
			GameResult = new GameEndAnalyzer().Analyze(_commandList, AnalysisInput);
			Performance = new PerformanceAnalyzer().Analyze(_commandList, AnalysisInput);
		}

		public bool Won(string code)
		{
			if (GameResult == null)
			{
				Analyze();
			}

			return GameResult.WinnerCode == code;
		}
	}
}
