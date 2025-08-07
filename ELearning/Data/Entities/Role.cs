using Data.Entities.Base;

namespace Data.Entities
{
    public class Role : AppEntityBase
    {
        public Role()
        {
            AppUsers = new HashSet<Users>();
            RolePermissions = new HashSet<RolePermission>();
        }
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool? CanDelete { get; set; }

        public ICollection<Users> AppUsers { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}