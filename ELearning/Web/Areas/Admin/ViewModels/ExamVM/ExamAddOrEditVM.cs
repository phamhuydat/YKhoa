using Data.Entities;

namespace Web.Areas.Admin.ViewModels.ExamVM
{
	public class ExamAddOrEditVM
	{
		public string Title { get; set; }
		public DateTime? TimeStart { get; set; }
		public DateTime? TimeEnd { get; set; }
		public int WorkTime { get; set; }
		public int SubjectId { get; set; }
		public bool IsAutomatic { get; set; }
		public bool MixQuestion { get; set; }
		public bool MixAnswer { get; set; }
		public bool SeeAnswer { get; set; }
		public bool SubmitWhenExit { get; set; }
		public int EQCount { get; set; }
		public int MQCount { get; set; }
		public int HQCount { get; set; }
		public bool Status { get; set; }
		public List<AutomaticExamVM> AutomaticExams { get; set; }
		public List<HandOutExamVM> HandOutExams { get; set; }

	}
}
