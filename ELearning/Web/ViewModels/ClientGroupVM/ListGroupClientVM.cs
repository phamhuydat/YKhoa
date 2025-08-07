using Web.Areas.Admin.ViewModels;

namespace Web.ViewModels.ClientGroupVM
{
    public class ListGroupClientVM : ListItemBaseVM
    {

        public string GroupName { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public int DisplayOrder { get; set; }

    }
}
