using Data.Entities.Base;

namespace Data.Entities
{
    public class RolePermission : AppEntityBase
    {
        public int RoleId { get; set; }
        public int MstPermissionId { get; set; }

        public Role Role { get; set; }
        public MstPermission MstPermission { get; set; }
    }
}