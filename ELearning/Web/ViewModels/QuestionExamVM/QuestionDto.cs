namespace Web.ViewModels.QuestionExamVM
{
	public class QuestionDto
	{
		public int QuestionNumber { get; set; }
		public string Title { get; set; }
		public string QuestionText { get; set; }
		public List<OptionDto> Options { get; set; } // Danh sách AnswerText từ Answers
	}
}
