namespace Web.Areas.Admin.ViewModels.GroupVM
{
	public class GroupAddOrEditVM : ListItemBaseVM
	{
		public string GroupName { get; set; }
		public string note { get; set; }
		public int SubjectId { get; set; }
		public string AcademicYear { get; set; }
		public int Semester { get; set; }
	}
}
