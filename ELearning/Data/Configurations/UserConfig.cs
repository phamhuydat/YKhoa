using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Share.Consts;

namespace Data.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.ToTable(DB.AppUser.TABLE_NAME);

            // Khóa chính
            builder.HasKey(m => m.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(m => m.MSSV)
              .HasMaxLength(DB.AppUser.USERNAME_LENGTH)
              .IsUnicode(false)   // varchar (không chứa unicode)
              .IsRequired();

            // Tên đăng nhập là varchar, bắt buộc & không trùng lặp
            builder.Property(m => m.FullName)
                .HasMaxLength(DB.AppUser.USERNAME_LENGTH)
                .IsRequired();

            builder.Property(m => m.Avatar)
                .HasMaxLength(DB.AppUser.AVATAR_LENGTH);

            builder.Property(m => m.Email)
                .HasMaxLength(DB.AppUser.EMAIL_LENGTH)
                .IsUnicode(false);

            builder.Property(m => m.FullName)
                .HasMaxLength(DB.AppUser.FULLNAME_LENGTH);

            builder.Property(m => m.Password)
                .HasMaxLength(DB.AppUser.PWD_LENGTH);

            // Khóa ngoại với AppRole
            builder.HasOne(m => m.Role)
                .WithMany(m => m.AppUsers)
                .HasForeignKey(m => m.AppRoleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Results)
                .WithOne(x => x.users)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}