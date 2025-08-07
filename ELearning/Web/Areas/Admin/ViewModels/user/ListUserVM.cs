using System.Runtime.CompilerServices;

namespace Web.Areas.Admin.ViewModels.user
{
    public class ListUserVM : ListItemBaseVM
    {
        public string MSSV { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public DateTime? BlockedTo { get; set; }
        public int AppRoleId { get; set; }

        public bool IsBlock
        {
            get
            {
                var now = DateTime.Now;
                if (BlockedTo.HasValue && BlockedTo > now)
                {
                    return true;
                }
                return false;
            }
        }

    }
}
