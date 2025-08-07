using Data.Entities;
using DocumentFormat.OpenXml.Office.CoverPageProps;

namespace Web.Areas.Admin.ViewModels.ExamVM
{
    public class ListExamVM : ListItemBaseVM
    {
        public string Title { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string ListGroup { get; set; }
        public bool IsAuto { get; set; }

        public int IsStatus
        {
            get
            {
                if (TimeStart > DateTime.Now)
                {
                    return 1;
                }
                if (TimeStart <= DateTime.Now && TimeEnd >= DateTime.Now)
                {
                    return 2;
                }
                return 3;
            }
        }
        public string TextColor
        {
            get
            {
                if (TimeStart > DateTime.Now)
                {
                    return "warning";
                }
                if (TimeStart <= DateTime.Now && TimeEnd >= DateTime.Now)
                {
                    return "success";
                }
                return "danger";
            }
        }
    }
}
