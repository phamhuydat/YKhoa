using Data.Entities;

namespace Web.Areas.Admin.ViewModels.NotifyVM
{
    public class AddOrEditNotifyVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public ICollection<NotifyDetailsVM> Details { get; set; }
    }
}
