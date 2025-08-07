namespace Web.Areas.Admin.ViewModels.ChapterVM
{
    public class ListChapterVM : ListItemBaseVM
    {
        public string ChapterName { get; set; }
        public bool Status { get; set; }
        public int SubjectId { get; set; }
    }
}
