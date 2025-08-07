using Data.Entities.Base;

namespace Data.Entities
{
	public class Answer : AppEntityBase
	{
		public Answer()
		{
			ResultDetails = new HashSet<ResultDetails>();
		}
		public string AnswerContent { get; set; }
		public bool Status { get; set; }
		//fk
		public int QuestionId { get; set; }
		public string Feedback { get; set; }
		public Question Question { get; set; }
		public ICollection<ResultDetails> ResultDetails { get; set; }
	}
}