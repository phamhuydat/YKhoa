using Data.Repositories;
using Share.Consts;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Components.MainNavBar;

namespace Web.Areas.Admin.Components.MainNavBar
{
    public class MainNavBarViewComponent : ViewComponent
    {

        readonly GenericRepository repository;
        public MainNavBarViewComponent(GenericRepository _repository)
        {
            repository = _repository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var navBar = new NavBarViewModel();
            navBar.Items.AddRange(new MenuItem[]
            {
                new MenuItem
                {
                    Action = "Index",
                    Controller = "Home",
                    DisplayText = "Trang chủ",
                    Icon = "tachometer-alt",
                    Permission = AuthConst.NO_PERMISSION
                },
                new MenuItem
                {
                    DisplayText = "Người dùng",
                    SidebarText = "users",
                    Icon = "users",
                    Permission = AuthConst.AppUser.VIEW_DETAIL,
                    ChildrenItems = new MenuItem[]
                    {
                        new MenuItem
                        {
                            Action = "Index",
                            Controller = "User",
                            DisplayText = "Danh sách người dùng",
                            Permission = AuthConst.AppUser.VIEW_DETAIL
                        },
                        new MenuItem
                        {
                            Action = "Index",
                            Controller = "Role",
                            DisplayText = "Quản lý phân quyền",
                            Permission = AuthConst.AppUser.UPDATE
                        }
                    }
                },

                new MenuItem
                {
                    Action = "Index",
                    Controller = "Group",
                    DisplayText = "Nhóm học phần",
                    SidebarText = "group",
                    Icon = "layer-group",
                    Permission = AuthConst.AppGroup.VIEW_DETAIL,
                },
                new MenuItem
                {
                    Action = "Index",
                    Controller = "Question",
                    DisplayText = "Câu hỏi",
                    SidebarText = "question",
                    Icon = "question-circle",
                    Permission = AuthConst.AppQuestion.VIEW_DETAIL,
                },
                new MenuItem
                {
                    Action = "Index",
                    Controller = "Subject",
                    DisplayText = "Môn học",
                    SidebarText = "subject",
                    Icon = "folder-open",
                    Permission = AuthConst.AppSubject.VIEW_DETAIL,
                },

                new MenuItem
                {
                    Action = "Index",
                    Controller = "Exam",
                    DisplayText = "Đề thi",
                    SidebarText = "Exam",
                    Icon = "file-alt",
                    Permission = AuthConst.AppExam.VIEW_DETAIL,
                },
                new MenuItem
                {
                    Action = "Index",
                    Controller = "Assignment",
                    DisplayText = "Phân công",
                    SidebarText = "Assignment",
                    Icon = "chalkboard-teacher",
                    Permission = AuthConst.AppExam.VIEW_DETAIL,
                },
                 new MenuItem
                {
                    Action = "Index",
                    Controller = "Notify",
                    DisplayText = "Thông báo",
                    SidebarText = "Notify",
                    Icon = "bell",
                    Permission = AuthConst.AppNotification.VIEW_DETAIL,
                },
            });
            return View(navBar);
        }
    }

}
