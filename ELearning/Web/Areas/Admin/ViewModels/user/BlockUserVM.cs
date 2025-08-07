namespace Web.Areas.Admin.ViewModels.user
{
    public class BlockUserVM
    {
        public int Id { get; set; }
        public DateTime? BlockedTo { get; set; }
        public bool Permanentblock { get; set; }
    }
}
