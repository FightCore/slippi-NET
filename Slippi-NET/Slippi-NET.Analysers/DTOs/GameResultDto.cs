namespace SlippiNET.Analysers.DTOs
{
	public class GameResultDto
	{
		public string WinnerCode { get; set; }

		public bool WasTimeout { get; set; }

		public bool IsLRAStart { get; set; }

		public string LRAStartInitiator { get; set; }
	}
}
