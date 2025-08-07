namespace Share.Consts
{
    public static class DB
    {

        public static class AppRole
        {
            public const string TABLE_NAME = "Role";
            public const short NAME_LENGTH = 100;
            public const short DESC_LENGTH = 100;
        }
        public static class AppRolePermission
        {
            public const string TABLE_NAME = "RolePermission";
        }
        public static class AppUser
        {
            public const string TABLE_NAME = "User";
            public const short USERNAME_LENGTH = 200;
            public const short PWD_LENGTH = 200;
            public const short FULLNAME_LENGTH = 50;
            public const short PHONE_LENGTH = 20;
            public const short EMAIL_LENGTH = 200;
            public const short ADDRESS_LENGTH = 100;
            public const short AVATAR_LENGTH = 250;
        }
        public static class MstPermission
        {
            public const string TABLE_NAME = "MstPermission";
            public const short CODE_LENGTH = 50;
            public const short TABLE_LENGTH = 50;
            public const short GROUPNAME_LENGHT = 100;
            public const short DESC_LENGHT = 100;
        }

        public static class AppMailSubscriber
        {
            public const string TABLE_NAME = "MailSubscriber";
            public const short EMAIL_LENGTH = 200;
        }
        public static class AppGroup
        {
            public const string TABLE_NAME = "Group";
            public const short NAME_LENGTH = 100;
            public const short DESC_LENGTH = 100;
            public const short CODE_LENGTH = 10;
            public const short NOTE_LENGTH = 100;
            public const short NAME_TEACHER_LENGTH = 100;
        }
        public static class AppAnswer
        {
            public const string TABLE_NAME = "Answer";
            public const short COTENT_LENGTH = 200;
            public const short DESC_LENGTH = 100;
        }

        public static class AppQuestion
        {
            public const string TABLE_NAME = "Question";
            public const short COTENT_LENGTH = 200;
            public const short DESC_LENGTH = 100;
        }
        public static class AppAssignment
        {
            public const string TABLE_NAME = "Assignment";
            public const short DESC_LENGTH = 100;
        }

        public static class AppChapter
        {
            public const string TABLE_NAME = "Chapter";
            public const short NAME_LENGTH = 50;
            public const short DESC_LENGTH = 100;
        }
        public static class AppExam
        {
            public const string TABLE_NAME = "Exam";
            public const short DESC_LENGTH = 100;
            public const short TITLE_LENGTH = 100;
        }

        public static class AppNotification
        {
            public const string TABLE_NAME = "Notification";
            public const short CONTENT_LENGTH = 100;
            public const short NAME_LENGTH = 100;
            public const short DESC_LENGTH = 100;

        }

        public static class AppSubject
        {
            public const string TABLE_NAME = "Subject";
            public const short DESC_LENGTH = 100;
            public const short NAME_LENGTH = 100;
        }
        public static class AppHandOutExam
        {
            public const string TABLE_NAME = "HandOutExam";

        }

    }
}
