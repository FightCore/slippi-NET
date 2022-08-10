using System;
using System.Collections.Generic;
using System.Linq;
using SlippiNET.Analyzers.DTOs;
using SlippiNET.Models.Commands;

namespace SlippiNET.Analyzers.Analyzers
{
	public class GameEndAnalyzer
	{
		public GameResultDto Analyze(List<BaseSlippiCommand> commands, AnalysisInputDto input)
		{
			var selfLastFrame =
				commands.Last(command => command is SlippiPostFrameUpdateCommand postFrameUpdateCommand
				                         && postFrameUpdateCommand.PlayerIndex == input.PlayerIndex) as
					SlippiPostFrameUpdateCommand;
			var opponentLastFrame =
				commands.Last(command => command is SlippiPostFrameUpdateCommand postFrameUpdateCommand
				                         && postFrameUpdateCommand.PlayerIndex == input.OpponentIndex) as
					SlippiPostFrameUpdateCommand;

			var won = selfLastFrame.StocksRemaining > opponentLastFrame.StocksRemaining;


			if (selfLastFrame.StocksRemaining != opponentLastFrame.StocksRemaining)
			{
				return new GameResultDto()
				{
					IsLRAStart = false,
					WasTimeout = false,
					WinnerCode = won ? input.PlayerCode : input.OpponentCode
				};
			}

			if (selfLastFrame.StocksRemaining != 1 && opponentLastFrame.StocksRemaining != 1)
			{
				Console.WriteLine("Game didn't end on one stock, most likely laggy quit.");
			}

			var gameEndCommand =
				commands.First(command => command is SlippiGameEndCommand) as SlippiGameEndCommand;
			if (gameEndCommand == null || gameEndCommand.LRASInitiator == -1)
			{
				throw new Exception("Game ended equal without LRAS indication, doesn't count.");
			}

			return new GameResultDto()
			{
				IsLRAStart = true,
				LRAStartInitiator = gameEndCommand.LRASInitiator == input.PlayerIndex
					? input.PlayerCode
					: input.OpponentCode,
				WinnerCode = gameEndCommand.LRASInitiator != input.PlayerIndex
					? input.PlayerCode
					: input.OpponentCode,
			};
		}
	}
}
