namespace Web.Areas.Admin.ViewModels.GroupVM
{
    public class ListGroupVM : ListItemBaseVM
    {
        public string Title { get; set; }
        public List<GroupDetailVM> ListItemGroup { get; set; } = new List<GroupDetailVM>();

    }


}
