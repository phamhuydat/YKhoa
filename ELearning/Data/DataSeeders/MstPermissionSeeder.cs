using Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Share.Consts;

namespace Data.DataSeeders
{
    public static class MstPermissionSeeder
    {
        public static void SeedData(this EntityTypeBuilder<MstPermission> builder)
        {
            var now = DateTime.Now;
            var groupName = "";

            #region Data liên quan đến bảng Role
            // Permission liên quan đến bảng AppRole
            groupName = "Quản lý phân quyền";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppRole.CREATE,
                    Code = "CREATE",
                    Table = DB.AppRole.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm quyền",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppRole.DELETE,
                    Code = "DELETE",
                    Table = DB.AppRole.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa quyền",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppRole.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppRole.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Sửa quyền",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppRole.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppRole.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết quyền",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppRole.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppRole.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách quyền",
                    CreatedDate = now
                }
            );
            #endregion
            #region Data liên quản bảng User
            // Permission liên quan đến bảng AppUser
            groupName = "Quản lý người dùng";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppUser.BLOCK,
                    Code = "BLOCK",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Khóa người dùng",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppUser.CREATE,
                    Code = "CREATE",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm người dùng",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppUser.DELETE,
                    Code = "DELETE",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa người dùng",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppUser.UNBLOCK,
                    Code = "UNBLOCK",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Mở khóa người dùng",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppUser.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Cập nhật người dùng",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppUser.UPDATE_PWD,
                    Code = "UPDATE_PWD",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Đổi mật khẩu",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppUser.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết người dùng",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppUser.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppUser.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách người dùng",
                    CreatedDate = now
                }
            );
            #endregion

            #region Data liên quan đến bảng câu trả lời
            groupName = "Quản lý trả lời";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppAnswer.CREATE,
                    Code = "CREATE",
                    Table = DB.AppAnswer.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm câu trả lời",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAnswer.DELETE,
                    Code = "DELETE",
                    Table = DB.AppAnswer.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa câu trả lời",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAnswer.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppAnswer.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Sửa câu trả lời",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAnswer.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppAnswer.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết câu trả lời",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAnswer.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppAnswer.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách câu trả lời",
                    CreatedDate = now
                }
            );
            #endregion
            #region Data liên quan đến bảng câu hỏi
            groupName = "Quản lý câu hỏi";
            builder.HasData(
                 new MstPermission
                 {
                     Id = AuthConst.AppQuestion.CREATE,
                     Code = "CREATE",
                     Table = DB.AppQuestion.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Thêm câu hỏi",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppQuestion.DELETE,
                     Code = "DELETE",
                     Table = DB.AppQuestion.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xóa câu hỏi",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppQuestion.UPDATE,
                     Code = "UPDATE",
                     Table = DB.AppQuestion.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Sửa câu hỏi",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppQuestion.VIEW_DETAIL,
                     Code = "VIEW_DETAIL",
                     Table = DB.AppQuestion.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xem chi câu hỏi",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppQuestion.VIEW_LIST,
                     Code = "VIEW_LIST",
                     Table = DB.AppQuestion.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xem danh sách câu hỏi",
                     CreatedDate = now
                 }
             );
            #endregion
            #region Data liên quan đến bảng đề thi
            groupName = "Quản lý đề thi";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppExam.CREATE,
                    Code = "CREATE",
                    Table = DB.AppExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm đề thi",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppExam.DELETE,
                    Code = "DELETE",
                    Table = DB.AppExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa đề thi",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppExam.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Sửa đề thi",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppExam.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết đề thi",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppExam.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách đề thi",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppExamStudent.JOIN,
                    Code = "JOIN",
                    Table = DB.AppExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Tham gia bài thi",
                    CreatedDate = now
                }
            );
            #endregion

            #region Data liên quan đến bảng môn học
            groupName = "Quản lý môn học";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppSubject.CREATE,
                    Code = "CREATE",
                    Table = DB.AppSubject.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm môn học",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppSubject.DELETE,
                    Code = "DELETE",
                    Table = DB.AppSubject.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa môn học",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppSubject.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppSubject.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Sửa môn học",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppSubject.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppSubject.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết môn học",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppSubject.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppSubject.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách môn học",
                    CreatedDate = now
                }
            );
            #endregion

            #region Data liên quan đến bảng nhóm lớp
            groupName = "Quản lý nhóm học phần";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppGroup.CREATE,
                    Code = "CREATE",
                    Table = DB.AppGroup.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm nhóm học phần",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppGroup.DELETE,
                    Code = "DELETE",
                    Table = DB.AppGroup.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa nhóm học phần",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppGroup.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppGroup.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Sửa nhóm học phần",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppGroup.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppGroup.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết nhóm học phần",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppGroup.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppGroup.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách nhóm học phần",
                    CreatedDate = now
                },
                 new MstPermission
                 {
                     Id = AuthConst.AppGroupStudent.JOIN,
                     Code = "JOIN",
                     Table = DB.AppGroup.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Tham gia bài thi",
                     CreatedDate = now
                 }
            );
            #endregion

            #region Data liên quan đến bảng phân công giảng viên
            groupName = "Quản lý phân công học phần";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppAssignment.CREATE,
                    Code = "CREATE",
                    Table = DB.AppAssignment.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm phân công",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAssignment.DELETE,
                    Code = "DELETE",
                    Table = DB.AppAssignment.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa phân công",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAssignment.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppAssignment.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Sửa phân công",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAssignment.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppAssignment.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết phân công",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppAssignment.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppAssignment.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách phân công",
                    CreatedDate = now
                }

            );
            #endregion

            #region Data liên quan đến bảng giao đề thi
            groupName = "Quản lý việc giao đề thi";
            builder.HasData(
                new MstPermission
                {
                    Id = AuthConst.AppHandOutExam.CREATE,
                    Code = "CREATE",
                    Table = DB.AppHandOutExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Thêm bài thi cho nhóm",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppHandOutExam.DELETE,
                    Code = "DELETE",
                    Table = DB.AppHandOutExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xóa xóa bài thi của nhóm",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppHandOutExam.UPDATE,
                    Code = "UPDATE",
                    Table = DB.AppHandOutExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Sửa bài thi đã giao 'vd: đổi bài thi, đổi nhóm thi'",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppHandOutExam.VIEW_DETAIL,
                    Code = "VIEW_DETAIL",
                    Table = DB.AppHandOutExam.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem chi tiết bài thi được giao cho nhóm",
                    CreatedDate = now
                },
                new MstPermission
                {
                    Id = AuthConst.AppHandOutExam.VIEW_LIST,
                    Code = "VIEW_LIST",
                    Table = DB.AppAssignment.TABLE_NAME,
                    GroupName = groupName,
                    Desc = "Xem danh sách bài thi đã giao cho nhóm nào",
                    CreatedDate = now
                }
            );
            #endregion

            #region Data liên quan đến bảng thông báo
            groupName = "Quản lý thông báo";
            builder.HasData(
                 new MstPermission
                 {
                     Id = AuthConst.AppNotification.CREATE,
                     Code = "CREATE",
                     Table = DB.AppNotification.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Thêm thông báo nhóm",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppNotification.DELETE,
                     Code = "DELETE",
                     Table = DB.AppNotification.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xóa thông báo",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppNotification.UPDATE,
                     Code = "UPDATE",
                     Table = DB.AppNotification.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Sửa thông báo",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppNotification.VIEW_DETAIL,
                     Code = "VIEW_DETAIL",
                     Table = DB.AppNotification.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xem chi tiết thông báo",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppNotification.VIEW_LIST,
                     Code = "VIEW_LIST",
                     Table = DB.AppNotification.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xem danh sách thông báo",
                     CreatedDate = now
                 }
             );
            #endregion
            #region Data liên quan đến bảng chapter
            groupName = "Quản lý chương trình môn học";
            builder.HasData(
                 new MstPermission
                 {
                     Id = AuthConst.AppChapter.CREATE,
                     Code = "CREATE",
                     Table = DB.AppChapter.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Thêm chương môn học",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppChapter.DELETE,
                     Code = "DELETE",
                     Table = DB.AppChapter.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xóa chương môn học",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppChapter.UPDATE,
                     Code = "UPDATE",
                     Table = DB.AppChapter.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Sửa chương môn học",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppChapter.VIEW_DETAIL,
                     Code = "VIEW_DETAIL",
                     Table = DB.AppChapter.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xem chi tiết chương môn học",
                     CreatedDate = now
                 },
                 new MstPermission
                 {
                     Id = AuthConst.AppChapter.VIEW_LIST,
                     Code = "VIEW_LIST",
                     Table = DB.AppChapter.TABLE_NAME,
                     GroupName = groupName,
                     Desc = "Xem danh sách chương môn học",
                     CreatedDate = now
                 }
             );
            #endregion
        }
    }
}