using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SlippiNET.Analysers.DTOs;
using SlippiNET.Models.Commands;
using SlippiNET.Utils;

namespace SlippiNET.Analysers
{
	public class SlippiGame
	{
		private readonly string _filePath;
		private List<BaseSlippiCommand> _commandList;

		public GameResultDto GameResult { get; private set; }

		public AnalysisInputDto AnalysisInput { get; set; }

		public SlippiGame(string filePath, string playerCode)
		{
			_filePath = filePath;
			AnalysisInput = new AnalysisInputDto()
			{
				PlayerCode = playerCode
			};
		}

		public void Analyse()
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

			AnalysisInput.PlayerIndex = metaData.Players[0].Names["code"] == AnalysisInput.PlayerCode ? 0 : 1;
			AnalysisInput.OpponentIndex = AnalysisInput.PlayerIndex == 0 ? 1 : 0;
			AnalysisInput.OpponentCode = metaData.Players[AnalysisInput.OpponentIndex].Names["code"];

			var commands = new SlippiFileReader().Read(binaryFile, fileType);
			var commandsAsList = commands.ToList();
			var selfLastFrame =
				commandsAsList.Last(command => command is SlippiPostFrameUpdateCommand postFrameUpdateCommand
											   && postFrameUpdateCommand.PlayerIndex == AnalysisInput.PlayerIndex) as
					SlippiPostFrameUpdateCommand;
			var opponentLastFrame =
				commandsAsList.Last(command => command is SlippiPostFrameUpdateCommand postFrameUpdateCommand
											   && postFrameUpdateCommand.PlayerIndex == AnalysisInput.OpponentIndex) as
					SlippiPostFrameUpdateCommand;

			var won = selfLastFrame.StocksRemaining > opponentLastFrame.StocksRemaining;


			if (selfLastFrame.StocksRemaining != opponentLastFrame.StocksRemaining)
			{
				GameResult = new GameResultDto()
				{
					IsLRAStart = false,
					WasTimeout = false,
					WinnerCode = won ? AnalysisInput.PlayerCode : AnalysisInput.OpponentCode
				};
				return;
			}

			if (selfLastFrame.StocksRemaining != 1 && opponentLastFrame.StocksRemaining != 1)
			{
				Console.WriteLine("Game didn't end on one stock, most likely laggy quit.");
			}

			var gameEndCommand =
					commandsAsList.First(command => command is SlippiGameEndCommand) as SlippiGameEndCommand;
			if (gameEndCommand == null || gameEndCommand.LRASInitiator == -1)
			{
				throw new Exception("Game ended equal without LRAS indication, doesn't count.");
			}

			GameResult = new GameResultDto()
			{
				IsLRAStart = true,
				LRAStartInitiator = gameEndCommand.LRASInitiator == AnalysisInput.PlayerIndex
					? AnalysisInput.PlayerCode
					: AnalysisInput.OpponentCode,
				WinnerCode = gameEndCommand.LRASInitiator != AnalysisInput.PlayerIndex
					? AnalysisInput.PlayerCode
					: AnalysisInput.OpponentCode,
			};
		}


		public bool Won(string code)
		{
			if (GameResult == null)
			{
				Analyse();
			}

			return GameResult.WinnerCode == code;
		}
	}
}
