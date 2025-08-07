using Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataSeeders
{
    public static class RoleSeeder
    {
        public static void SeedData(this EntityTypeBuilder<Role> builder)
        {
            var now = DateTime.Now;

            // Tạo vai trò
            var roleCustomer = new Role
            {
                Id = 1,
                Name = "Student",
                Desc = "Sinh Viên",
                CreatedDate = now,
                UpdatedDate = now,
                CanDelete = false
            };

            var roleAdmin = new Role
            {
                Id = 2,
                Name = "Admin",
                Desc = "Quản trị toàn bộ hệ thống",
                CreatedDate = now,
                UpdatedDate = now,
                CanDelete = true
            };
            var roleTeacher = new Role
            {
                Id = 3,
                Name = "Teacher",
                Desc = "giáo viên",
                CreatedDate = now,
                UpdatedDate = now,
                CanDelete = true
            };

            builder.HasData(roleCustomer, roleAdmin, roleTeacher);
        }
    }
}