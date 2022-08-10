namespace SlippiNET.Analyzers.DTOs.Performance
{
	public class LCancelPerformanceDto
	{
		public int Success { get; }

		public int Failed { get; }

		public int Total => Success + Failed;

		public double LCancelPercentage => Total > 0 ? (double) Success / Total * 100 : 0;

		public LCancelPerformanceDto(int success, int failed)
		{
			Success = success;
			Failed = failed;
		}
	}
}
