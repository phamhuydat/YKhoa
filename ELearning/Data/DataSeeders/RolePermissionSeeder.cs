using Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Share.Consts;
using System.Reflection;

namespace Data.DataSeeders
{
    public static class RolePermissionSeeder
    {
        private static int i = 0;

        public static void SeedData(this EntityTypeBuilder<RolePermission> builder)
        {
            var now = DateTime.Now;

            // Danh sách các class chứa permission cho quản trị viên
            Type[] classTypeAdmin = new Type[]
            {
                typeof(AuthConst.AppRole),
                typeof(AuthConst.AppUser),
                typeof(AuthConst.AppExam),
                typeof(AuthConst.AppAnswer),
                typeof(AuthConst.AppQuestion),
                typeof(AuthConst.AppNotification),
                typeof(AuthConst.AppGroup),
                typeof(AuthConst.AppChapter),
                typeof(AuthConst.AppSubject),
                typeof(AuthConst.AppHandOutExam),
                typeof(AuthConst.AppAssignment),
            };

            // Cấp quyền cho vai trò admin
            var rolePermissionList = new List<RolePermission>();


            foreach (var type in classTypeAdmin)
            {
                var allPermission = GetConstants(type);
                foreach (var permission in allPermission)
                {
                    i++;
                    rolePermissionList.Add(new RolePermission
                    {
                        Id = i,
                        MstPermissionId = Convert.ToInt32(permission.GetRawConstantValue()),
                        UpdatedDate = now,
                        CreatedDate = now,
                        RoleId = 2,      // Vai trò được tạo ở AppRoleSeeder vai trò quản trị
                    });
                }
            }

            // Danh sách các class chứa permission cho sinh viên
            Type[] classTypeStudent = new Type[]
            {
                typeof(AuthConst.AppExamStudent),
                typeof(AuthConst.AppGroupStudent),
            };

            // Cấp quyền cho sinh viên (sửa lỗi dùng sai classTypeTeacher)
            foreach (var type in classTypeStudent)
            {
                var allPermission = GetConstants(type);
                foreach (var permission in allPermission)
                {
                    i++;
                    rolePermissionList.Add(new RolePermission
                    {
                        Id = i,
                        MstPermissionId = Convert.ToInt32(permission.GetRawConstantValue()),
                        UpdatedDate = now,
                        CreatedDate = now,
                        RoleId = 1,      // Vai trò được tạo ở AppRoleSeeder vai trò sinh viên
                    });
                }
            }

            // Danh sách các class chứa permission cho giáo viên
            Type[] classTypeTeacher = new Type[]
            {
                typeof(AuthConst.AppExam),
                typeof(AuthConst.AppAnswer),
                typeof(AuthConst.AppQuestion),
                typeof(AuthConst.AppNotification),
                typeof(AuthConst.AppGroup),
                typeof(AuthConst.AppChapter),
                typeof(AuthConst.AppSubject),
                typeof(AuthConst.AppHandOutExam),
                typeof(AuthConst.AppAssignment),
            };

            // Cấp quyền cho giáo viên
            foreach (var type in classTypeTeacher)
            {
                var allPermission = GetConstants(type);
                foreach (var permission in allPermission)
                {
                    i++;
                    rolePermissionList.Add(new RolePermission
                    {
                        Id = i,
                        MstPermissionId = Convert.ToInt32(permission.GetRawConstantValue()),
                        UpdatedDate = now,
                        CreatedDate = now,
                        RoleId = 3,      // Vai trò được tạo ở AppRoleSeeder vai trò giáo viên
                    });
                }
            }

            // Truyền dữ liệu vào builder
            builder.HasData(rolePermissionList);
        }

        private static List<FieldInfo> GetConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }
    }

}