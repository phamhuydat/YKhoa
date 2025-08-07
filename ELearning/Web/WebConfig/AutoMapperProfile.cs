using AutoMapper;
using Data.Entities;
using Web.Areas.Admin.ViewModels.Account;
using Web.Areas.Admin.ViewModels.AnswerVM;
using Web.Areas.Admin.ViewModels.AssignmentVM;
using Web.Areas.Admin.ViewModels.ChapterVM;
using Web.Areas.Admin.ViewModels.ExamVM;
using Web.Areas.Admin.ViewModels.GroupDetailVM;
using Web.Areas.Admin.ViewModels.GroupVM;
using Web.Areas.Admin.ViewModels.NotifyVM;
using Web.Areas.Admin.ViewModels.QuestionVM;
using Web.Areas.Admin.ViewModels.Role;
using Web.Areas.Admin.ViewModels.SubjectVM;
using Web.Areas.Admin.ViewModels.user;
using Web.ViewModels.Account;
using Web.ViewModels.ClientExamVM;
using Web.ViewModels.ClientGroupVM;
using Web.ViewModels.QuestionExamVM;

namespace Web.WebConfig
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            // Map dữ liệu từ kiểu AppUser sang UserAddOrEditVM

            CreateMap<Users, UpdateUserViewModel>().ReverseMap();

            CreateMap<Users, AcceptUpdateViewModel>().ReverseMap();

            // map dl từ UserAddOrEditVM xang User
            CreateMap<UserAddOrEditVM, Users>().ReverseMap();

            //map dl từ subjectUpsertVM xang subject
            CreateMap<SubjectAddOrUpdateVM, Subject>().ReverseMap();

            //map dl từ chapterUpsertVM xang chapter
            CreateMap<ChapterAddOrEditVM, Chapter>().ReverseMap();

            // Map dữ liệu từ AnswerAddOrEdit sang Answer
            CreateMap<AnswerAddOrEdit, Answer>().ReverseMap();

            // map dữ liệu QuestionAddOrEditVM sang Question
            CreateMap<QuestionAddOrEditVM, Question>()
                .ForMember(q => q.answers, opts => opts.MapFrom(qVM => qVM.Options))
                .ReverseMap();

            // map dữ liệu từ group sang ListGroupVM
            CreateMap<Group, ListGroupVM>().ReverseMap();

            // map dữ liệu từ groupAddOrEditVM sang group
            CreateMap<GroupAddOrEditVM, Group>().ReverseMap();

            // map dữ liệu từ ExamAddOrEditVM sang Exam
            CreateMap<ExamAddOrEditVM, Exam>().ReverseMap();
            CreateMap<Exam, ExamAddOrEditVM>().ReverseMap();

            //map dữ liệu từ automaticExamVM sang automaticExam
            CreateMap<AutomaticExamVM, AutomaticExam>().ReverseMap();
            CreateMap<AutomaticExam, AutomaticExamVM>().ReverseMap();

            //map dữ liệu từ handoutExamVM sang handoutExam
            CreateMap<HandOutExamVM, HandOutExam>().ReverseMap();
            CreateMap<HandOutExam, HandOutExamVM>().ReverseMap();



            CreateMap<Exam, ListExamUserVM>()
                .ForMember(vm => vm.ExamName, opts => opts.MapFrom(entity => entity.Title))
                .ForMember(vm => vm.SubjectName, opts => opts.MapFrom(entity =>
                    entity.handOutExams.First().group.subject.SubjectName + " - NH"
                    + entity.handOutExams.First().group.AcademicYear + " - HK"
                    + entity.handOutExams.First().group.Semester))
                .ForMember(vm => vm.WorkTime, opts => opts.MapFrom(entity => entity.WorkTime))
                .ForMember(vm => vm.StartTime, opts => opts.MapFrom(entity => entity.TimeStart))
                .ForMember(vm => vm.EndTime, opts => opts.MapFrom(entity => entity.TimeEnd))
                .ForMember(vm => vm.TotalQuestion, opts => opts.MapFrom(entity => entity.EQCount + entity.MQCount + entity.HQCount))
                .ReverseMap();


            CreateMap<Question, Question>().ReverseMap();


            // map dữ liệu từ AddOrEditNotifyVM sang Notification
            CreateMap<AddOrEditNotifyVM, Notification>().ReverseMap();
            CreateMap<Notification, AddOrEditNotifyVM>().ReverseMap();

            // map dữ liệu từ NotifyDetailsVM sang NotificationDetails
            CreateMap<NotifyDetailsVM, NotificationDetails>().ReverseMap();
            CreateMap<NotificationDetails, NotifyDetailsVM>().ReverseMap();

            // map dữ liệu từ AddOrEditAssignmentVM sang Assignment
            CreateMap<AddOrEditAssignmentVM, Assignment>()
                .ForMember(vm => vm.SubjectId, opts => opts.MapFrom(entity => entity.SubjectId))
                .ForMember(vm => vm.UserId, opts => opts.MapFrom(entity => entity.UserId))
                .ReverseMap();

        }

        public static MapperConfiguration RoleIndexConf = new(mapper =>
        {
            // Map dữ liệu từ kiểu AppRole sang RoleListItemVM
            mapper.CreateMap<Role, RoleListItemVM>();
        });
        // Cấu hình mapping cho RoleController, action Delete
        public static MapperConfiguration RoleDeleteConf = new(mapper =>
        {
            // Map dữ liệu thuộc tính con
            mapper.CreateMap<Users, RoleDeleteVM_User>();
            // Map dữ liệu thuộc tính cha
            mapper.CreateMap<Role, RoleDeleteVM>();
        });

        // Cấu hình mapping cho UserController, action Index
        public static MapperConfiguration UserIndexConf = new(mapper =>
        {
            // Map dữ liệu từ AppUser sang UserListItemVM, map thuộc tính RoleName
            mapper.CreateMap<Users, ListUserVM>()
                .ForMember(uItem => uItem.RoleName, opts => opts.MapFrom(uEntity => uEntity.Role.Name)).ReverseMap();

        });


        public static MapperConfiguration LoginConf = new(mapper =>
        {
            // Map dữ liệu từ AppUser sang UserListItemVM, map thuộc tính RoleName
            mapper.CreateMap<Users, UserDataForApp>()
                .ForMember(uItem => uItem.RoleName, opts => opts.MapFrom(uEntity => uEntity.Role == null ? "" : uEntity.Role.Name))
                .ForMember(uItem => uItem.Permission, opts => opts.MapFrom
                (
                    uEntity => string.Join(',', uEntity.Role
                                                        .RolePermissions
                                                        .Select(p => p.MstPermissionId))
                )).ReverseMap();
        });


        // cấu hình mapper cho ExamController, action Index
        public static MapperConfiguration ExamIndexConf = new MapperConfiguration(mapper =>
        {
            mapper.CreateMap<Exam, ListExamVM>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TimeStart, opt => opt.MapFrom(src => src.TimeStart))
                .ForMember(dest => dest.TimeEnd, opt => opt.MapFrom(src => src.TimeEnd))
                .ForMember(dest => dest.IsAuto, opt => opt.MapFrom(src => src.IsAutomatic))
                .ForMember(dest => dest.ListGroup, opt => opt.MapFrom
                (
                    uEntity => string.Join(", ", uEntity.handOutExams.Select(p => p.group.GroupName))
                ))

                .ReverseMap();
        });


        // Cấu hình mapping cho QuestionController, action Index AppQuestion xang ListQuestionVM
        public static MapperConfiguration QuestionIndexConf = new(mapper =>
        {
            mapper.CreateMap<Question, ListQuestionVM>()
                .ForMember(qItem => qItem.SubjectName, opts => opts.MapFrom(qEntity => qEntity.subject.SubjectName))
                .ForMember(qItem => qItem.ChapterName, opts => opts.MapFrom(qEntity => qEntity.chapter.ChapterName))
                .ForMember(qItem => qItem.Answers, opts => opts.MapFrom(qEntity => qEntity.answers)).ReverseMap();
        });

        // cấu hình mapping cho QuestionController, Question xang QuestionAddOrEditVM
        public static MapperConfiguration QuestionAddOrEditConf = new(mapper =>
        {
            mapper.CreateMap<Question, QuestionAddOrEditVM>()
                .ForMember(qItem => qItem.SubjectId, opts => opts.MapFrom(qEntity => qEntity.SubjectId))
                .ForMember(qItem => qItem.ChapterId, opts => opts.MapFrom(qEntity => qEntity.ChapterId))
                .ForMember(qItem => qItem.Level, opts => opts.MapFrom(qEntity => qEntity.Level))
                .ForMember(qItem => qItem.Options, opts => opts.MapFrom(qEntity => qEntity.answers)).ReverseMap();
        });

        // Cấu hình mapping cho SubjectController, action Index

        public static MapperConfiguration SubjectIndexConf = new(mapper =>
        {
            mapper.CreateMap<Subject, ListSubjectVM>().ReverseMap();
        });

        // Cấu hình mapping cho ChapterController, action Index
        public static MapperConfiguration ChapterIndexConf = new(mapper =>
        {
            mapper.CreateMap<Chapter, ListChapterVM>().ReverseMap();
        });

        // Cấu hình mapping cho AnswerController, action Index
        public static MapperConfiguration AnswerIndexConf = new(mapper =>
        {
            mapper.CreateMap<Answer, AnswerAddOrEdit>().ReverseMap();
        });


        // Cấu hình mapping cho GroupController, action Index
        public static MapperConfiguration GroupIndexConf = new(mapper =>
        {
            mapper.CreateMap<Group, ListGroupVM>()
                .ForMember(vm => vm.Title, opts => opts.MapFrom(entity =>
                    $"{entity.subject.SubjectCode} - {entity.subject.SubjectName} - NH{entity.AcademicYear} - {entity.Semester}"))
                .ForMember(vm => vm.ListItemGroup, opts => opts.MapFrom(entity => new List<GroupDetailVM>
                {
                    new GroupDetailVM
                    {
                        Id = entity.Id,
                        Notes = entity.Note,
                        Quantity = entity.GroupDetails.Count,
                        Name = entity.Teacher != null ? "GV: " + entity.Teacher : entity.GroupName,

                    }
                })).ReverseMap();

            mapper.CreateMap<Group, GroupDetailVM>()
                .ForMember(vm => vm.Id, opts => opts.MapFrom(entity => entity.Id))
                //.ForMember(vm => vm.GroupName, opts => opts.MapFrom(entity => entity.GroupName))
                .ForMember(vm => vm.Notes, opts => opts.MapFrom(entity => entity.Note))
                //.ForMember(vm => vm.Visibility, opts => opts.MapFrom(entity => entity.Visibility))
                .ReverseMap();
        });


        // cấu hình mapper cho GroupDetailController, action ListUserGroup
        public static MapperConfiguration GroupDetailIndexConf => new MapperConfiguration(mapper =>
        {
            mapper.CreateMap<GroupDetails, ListUserGroupVM>()
                .ForMember(dest => dest.Mssv, opt => opt.MapFrom(src => src.User.MSSV))
                .ForMember(dest => dest.fullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.User.Birthday))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ReverseMap();
        });


        public static MapperConfiguration GroupIndexClientConf => new MapperConfiguration(mapper =>
        {
            // map dữ liệu từ group xang listclientgroupvm
            mapper.CreateMap<Group, ListGroupClientVM>()
                .ForMember(vm => vm.GroupName, opts => opts.MapFrom(entity => entity.GroupName))
                .ForMember(vm => vm.TeacherName, opts => opts.MapFrom(entity => entity.Teacher != null ? "GV: " + entity.Teacher : "Chưa phân giảng viên"))
                .ForMember(vm => vm.SubjectName, opts => opts.MapFrom(entity => entity.subject.SubjectName))
                .ForMember(vm => vm.AcademicYear, opts => opts.MapFrom(entity => entity.AcademicYear))
                .ForMember(vm => vm.Semester, opts => opts.MapFrom(entity => entity.Semester))
                .ForMember(vm => vm.DisplayOrder, opts => opts.MapFrom(entity => entity.DisplayOrder))
                .ReverseMap();
        });

        public static MapperConfiguration GroupDetailIndexClientConf => new MapperConfiguration(mapper =>
        {
            // map dữ liệu từ group xang listclientgroupvm
            mapper.CreateMap<GroupDetails, ListUserInGroup>()
                .ForMember(vm => vm.FullName, opts => opts.MapFrom(entity => entity.User.FullName))
                .ReverseMap();
        });

        public static MapperConfiguration ExamIndexClientConf => new MapperConfiguration(mapper =>
        {
            // map dữ liệu từ exam xang listexamUserVM
            mapper.CreateMap<Exam, ListExamUserVM>()
                .ForMember(vm => vm.ExamName, opts => opts.MapFrom(entity => entity.Title))
                .ForMember(vm => vm.StartTime, opts => opts.MapFrom(entity => entity.TimeStart))
                .ForMember(vm => vm.EndTime, opts => opts.MapFrom(entity => entity.TimeEnd))
                .ForMember(vm => vm.SubjectName, opts => opts.MapFrom(entity =>
                    entity.handOutExams.First().group.subject.SubjectName + " - NH"
                    + entity.handOutExams.First().group.AcademicYear + " - HK"
                    + entity.handOutExams.First().group.Semester))
                .ReverseMap();
        });

        // cấu hình mapper cho TestController, TakeExam với ResQuestionVM
        public static MapperConfiguration ExamDetailsConf => new MapperConfiguration(mapper =>
        {
            // map dữ liệu từ group xang listclientgroupvm
            mapper.CreateMap<ExamDetails, ResQuestionVM>()
                .ForMember(vm => vm.Content, opts => opts.MapFrom(entity => entity.Question.Content))
                .ForMember(vm => vm.answers, opts => opts.MapFrom(entity => entity.Question.answers))
                .ReverseMap();
        });


        // cau hinh mapper cho NotificationController, Index
        public static MapperConfiguration NotificationIndexConf = new(mapper =>
        {
            mapper.CreateMap<Notification, ListNoifyVM>()
                .ForMember(vm => vm.Content, opts => opts.MapFrom(entity => entity.Content))
                .ForMember(vm => vm.CreateName, opts => opts.MapFrom(entity => entity.CreateName))
                .ForMember(vm => vm.CreateDate, opts => opts.MapFrom(entity => entity.CreatedDate))
                .ForMember(dest => dest.ListGroup, opt => opt.MapFrom
                (
                    uEntity => string.Join(", ", uEntity.NotificationDetailsDetails.Select(p => p.Group.GroupName))
                ))
                .ReverseMap();
        });
        public static MapperConfiguration GetListAssignmentConf = new(mapper =>
        {
            mapper.CreateMap<Assignment, ListAssignmentVM>()
                .ForMember(vm => vm.NameTeacher, opts => opts.MapFrom(entity => entity.Users.FullName))
                .ForMember(vm => vm.NameSubject, opts => opts.MapFrom(entity => entity.Subject.SubjectName))
                .ForMember(vm => vm.SubjectCode, opts => opts.MapFrom(entity => entity.Subject.SubjectCode))
                .ReverseMap();
        });


    }
}
