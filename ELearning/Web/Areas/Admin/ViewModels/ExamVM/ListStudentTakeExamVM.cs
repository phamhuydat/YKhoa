namespace Web.Areas.Admin.ViewModels.ExamVM
{
    public class ListStudentTakeExamVM : ListItemBaseVM
    {
        public string Mssv { get; set; }
        public int GroupId { get; set; }
        public int ExamId { get; set; }
        public string GroupName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public double? TestScores { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int? TotalWorkingTime { get; set; }
        public int? NumTSC { get; set; }

        public int Status
        {
            get
            {
                if (StartTime != null && EndTime == null)
                {
                    return 0; // Đang làm bài chưa nộp bài
                }
                else if (StartTime != null && EndTime != null)
                {
                    return 1; // Đã nộp bài
                }
                else
                {
                    return 2; // Chưa làm bài
                }

            }

        }
        public static List<double> GetScores(List<ListStudentTakeExamVM> students)
        {
            return students
                .Where(student => student.Status == 1)
                .Select(student => student?.TestScores ?? 0)
                .ToList();
        }




    }
}
