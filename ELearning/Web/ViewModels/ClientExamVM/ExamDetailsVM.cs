namespace Web.ViewModels.ClientExamVM
{
	public class ExamDetailsVM
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public int ResultId { get; set; }
		public int WorkTime { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
