namespace Web.Areas.Admin.ViewModels.GroupDetailVM
{
    public class ListUserGroupVM : ListItemBaseVM
    {

        public string GroupName { get; set; }
        public string Mssv { get; set; }
        public string fullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }

    }
}
