using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Share.Consts;

namespace Data.Configurations
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(DB.AppRole.TABLE_NAME);

            // Khóa chính
            builder.HasKey(m => m.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(m => m.Name)
                .HasMaxLength(DB.AppRole.NAME_LENGTH)
                .IsRequired();

            builder.Property(m => m.Desc)
                .HasMaxLength(DB.AppRole.DESC_LENGTH)
                .IsRequired();
        }
    }
}