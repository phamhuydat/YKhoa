using Data.Entities.Base;

namespace Data.Entities
{
	public class ResultDetails : AppEntityBase
	{
		public int ResultId { get; set; }
		public int QuestionId { get; set; }
		public int? AnswerId { get; set; }

		//fk
		public Result Result { get; set; }
		public Question Question { get; set; }
		public Answer Answer { get; set; }
	}
}
