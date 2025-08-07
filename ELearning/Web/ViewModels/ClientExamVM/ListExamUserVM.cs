using DocumentFormat.OpenXml.Office.CoverPageProps;
using Web.Areas.Admin.ViewModels;

namespace Web.ViewModels.ClientExamVM
{
    public class ListExamUserVM : ListItemBaseVM
    {
        public int Id { get; set; }
        public string ExamName { get; set; }
        public string SubjectName { get; set; }
        public int WorkTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalQuestion { get; set; }
        public int? TotalCorrectAnswer { get; set; }
        public double TotalScore { get; set; }
        public bool SeeAnswer { get; set; }
        public int? TotalWorkTime { get; set; }
        public DateTime? UserStartTime { get; set; }
        public DateTime? UserEndTime { get; set; }


        public int IsStatus
        {
            get
            {
                bool time = false;

                if (UserStartTime != null)
                {

                    TimeSpan tem = (TimeSpan)(DateTime.Now - UserStartTime);
                    time = WorkTime * 60 >= tem.Seconds ? true : false;
                }

                if (DateTime.Now < StartTime)
                {
                    return 0;  // baì thi chưa mở
                }
                else if (DateTime.Now > EndTime)
                {
                    return 2; // đã hết thời gian làm bài
                }
                else if (UserStartTime != null && StartTime < DateTime.Now && DateTime.Now < EndTime && UserEndTime == null)
                {
                    return 3; // tiếp tục làm bài
                }
                else if (UserEndTime != null)
                {
                    return 4; // đã hoàn thành bài thi
                }
                else
                {
                    return 1; // bai thi trang trong trạng thái mở
                }
            }
        }
        public string TotalWorkTimeInMinutes
        {
            get
            {
                if (TotalWorkTime.HasValue)
                {
                    int minutes = TotalWorkTime.Value / 60;
                    int seconds = TotalWorkTime.Value % 60;
                    return $"{minutes} phút {seconds} giây";
                }
                return "0";
            }
        }

    }
}
