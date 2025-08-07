using Data.Entities.Base;

namespace Data.Entities
{
    public class MstPermission : MstEntityBase
    {
        public MstPermission()
        {
            AppRolePermissions = new HashSet<RolePermission>();
        }
        public string Code { get; set; }
        public string Table { get; set; }
        public string GroupName { get; set; }
        public string Desc { get; set; }

        public ICollection<RolePermission> AppRolePermissions { get; set; }
    }
}