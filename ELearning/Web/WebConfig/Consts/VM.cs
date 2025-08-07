namespace Web.WebConfig
{
    public static class VM
    {
        public static class UserVM
        {
            public const short USERNAME_MINLEN = 4;
            public const short PWD_MINLEN = 4;
        }
        public static class RoleVM
        {
            public const string NEWID_REQUIRED_ERR_MESG = "Phải chọn vai trò mới để chuyển đổi";
            public const string PERMISSION_IDS_REQUIRED_ERR_MESG = "Chưa chọn quyền cho vai trò này";

            // Regex check chuỗi có đúng định dạng hay không (VD: 1111,2222,1002)
            public const string PERMISSION_IDS_REGEX = @"((?<!^,)\d+(,(?!$)|$))+";
            public const string PERMISSION_IDS_REGEX_ERR_MESG = "Dữ liệu không hợp lệ, hãy thử làm mới trang";
        }
        public static class AppNewsCategory
        {
            public const short MIN_LENGTH = 4;
            public const short MAX_LENGTH = 300;
        }
        public static class ProductCategoryVM
        {
            public const short MIN_LENGTH = 1;
        }
    }
}
