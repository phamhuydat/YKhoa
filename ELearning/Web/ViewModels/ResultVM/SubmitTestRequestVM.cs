namespace Web.ViewModels.ResultVM
{
    public class SubmitTestRequestVM
    {
        public int ResultId { get; set; }

        public int ExamId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int SubjectId { get; set; }
        public int ChapterId { get; set; }
        public int Level { get; set; }
        public int TotalQuestion { get; set; }
        public SubmitTestRequestVM() { }


    }
}
