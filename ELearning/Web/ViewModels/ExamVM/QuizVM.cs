using Data.Entities;

namespace Web.ViewModels.ExamVM
{
	public class QuizVM
	{
		public Question question { get; set; }
		public int CurrentQuestionIndex { get; set; }

	}
}
