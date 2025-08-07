using System.Runtime.CompilerServices;
using Web.Areas.Admin.ViewModels;

namespace Web.ViewModels.ClientGroupVM
{
    public class ListExamInGroupVM : ListItemBaseVM
    {

        public string Title { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string NameExam { get; set; }
        public int IsStatus
        {
            get
            {
                if (DateTime.Now < TimeStart)
                {
                    return 0;
                }
                else if (DateTime.Now > TimeEnd)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }

    }
}
