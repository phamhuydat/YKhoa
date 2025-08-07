using Data.Entities.Base;

namespace Data.Entities
{
	public class Result : AppEntityBase
	{
		public Result()
		{
			ResultDetails = new HashSet<ResultDetails>();
		}
		public double TestScores { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public int TotalWorkTime { get; set; }
		public int NumCorrect { get; set; } // NumOfCorrect
		public int NumTSC { get; set; }    // Num Of Tab Switches
		public int CurrentQuestion { get; set; } // Current Question Number

		// fk
		public int ExamId { get; set; }
		public int UserId { get; set; }
		public Exam exam { get; set; }
		public Users users { get; set; }
		public ICollection<ResultDetails> ResultDetails { get; set; }
	}
}
