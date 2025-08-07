using BCrypt.Net;
using Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.DataSeeders
{
	public static class UserSeeder
	{
		public static void SeedData(this EntityTypeBuilder<Users> builder)
		{
			var now = DateTime.Now;

			// Tạo mật khẩu
			var defaultPassword = "1111";
			var password = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

			// Tạo thông tin tài khoản admin
			builder.HasData(
				new Users
				{
					Id = 1,
					MSSV = "102024",
					Password = password,
					Email = "hello@gmail.com",
					FullName = "Admin",
					Phone = "0928666158",
					Avatar = "~/Images/Avatar/default.png",
					Gender = "Nam",
					CreatedBy = -1,
					UpdatedBy = -1,
					UpdatedDate = now,
					CreatedDate = now,
					AppRoleId = 2,              // Vai trò được tạo ở AppRoleSeeder
				}
			);
			builder.HasData(
				new Users
				{
					Id = 2,
					MSSV = "2110576",
					Password = password,
					Email = "codetoanbug@gmail.com",
					FullName = "Nguyen Van A",
					Phone = "0928666158",
					Avatar = "~/Images/Avatar/default.png",
					Gender = "Nam",
					CreatedBy = -1,
					UpdatedBy = -1,
					UpdatedDate = now,
					CreatedDate = now,
					AppRoleId = 1,              // Vai trò được tạo ở AppRoleSeeder
				}
			);
		}
	}
}