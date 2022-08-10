using System.Collections.Generic;
using SlippiNET.Analyzers.DTOs;
using SlippiNET.Analyzers.DTOs.Performance;
using SlippiNET.Models.Commands;

namespace SlippiNET.Analyzers.Analyzers.Performance
{
	public class PerformanceAnalyzer
	{
		public PerformanceDto Analyze(List<BaseSlippiCommand> commands, AnalysisInputDto input)
		{
			return new PerformanceDto()
			{
				LCancel = new LCancelPerformanceAnalyzer().Analyze(commands, input)
			};
		}
	}
}
