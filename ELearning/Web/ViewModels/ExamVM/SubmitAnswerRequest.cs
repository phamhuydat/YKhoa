namespace Web.ViewModels.ExamVM
{
	public class SubmitAnswerRequest
	{
		public int QuestionId { get; set; }
		public List<int> SelectAnswer { get; set; }
	}
}
