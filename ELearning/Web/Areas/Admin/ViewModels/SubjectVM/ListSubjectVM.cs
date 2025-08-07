namespace Web.Areas.Admin.ViewModels.SubjectVM
{
    public class ListSubjectVM : ListItemBaseVM
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int Credit { get; set; }
        public int NumTheory { get; set; } // 
        public int NumPractice { get; set; }
    }
}
