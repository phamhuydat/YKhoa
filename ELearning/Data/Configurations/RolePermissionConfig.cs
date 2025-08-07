using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Share.Consts;

namespace Data.Configurations
{
    public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable(DB.AppRolePermission.TABLE_NAME);

            // Khóa chính
            builder.HasKey(m => m.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // Khóa ngoại
            builder.HasOne(m => m.Role)
                .WithMany(m => m.RolePermissions)
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.MstPermission)
                .WithMany(m => m.AppRolePermissions)
                .HasForeignKey(m => m.MstPermissionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}