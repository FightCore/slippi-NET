using System.Collections.Generic;
using System.Linq;
using SlippiNET.Analyzers.DTOs;
using SlippiNET.Analyzers.DTOs.Performance;
using SlippiNET.Models;
using SlippiNET.Models.Commands;

namespace SlippiNET.Analyzers.Analyzers.Performance
{
	public class LCancelPerformanceAnalyzer
	{
		public LCancelPerformanceDto Analyze(List<BaseSlippiCommand> commands, AnalysisInputDto input)
		{
			return new LCancelPerformanceDto(commands.Count(command =>
			{
				if (command is not SlippiPostFrameUpdateCommand postFrameUpdateCommand)
				{
					return false;
				}

				if (postFrameUpdateCommand.PlayerIndex != input.PlayerIndex)
				{
					return false;
				}

				return postFrameUpdateCommand.LCancelStatus == 1;
			}), commands.Count(command =>
			{
				if (command is not SlippiPostFrameUpdateCommand postFrameUpdateCommand)
				{
					return false;
				}

				if (postFrameUpdateCommand.PlayerIndex != input.PlayerIndex)
				{
					return false;
				}

				return postFrameUpdateCommand.LCancelStatus == 2;
			}));
		}
	}
}
