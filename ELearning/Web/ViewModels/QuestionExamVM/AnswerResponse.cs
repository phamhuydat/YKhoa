namespace Web.ViewModels.QuestionExamVM
{
	public class AnswerResponse
	{
		public bool IsCorrect { get; set; }
		public string Explanation { get; set; }
		public int? CorrectOption { get; set; }
	}
}
