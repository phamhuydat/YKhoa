using Share.Consts;
using Shared.Attributes;
using System.ComponentModel.DataAnnotations;
using Web.WebConfig;

namespace Web.Areas.Admin.ViewModels.user
{
    public class UserAddOrEditVM
    {
        public int Id { get; set; }

        [AppRequired]
        [AppUsername]
        [AppStringLength(VM.UserVM.USERNAME_MINLEN, DB.AppUser.USERNAME_LENGTH)]
        public string Mssv { get; set; }

        [AppRequired]
        [DataType(DataType.Password)]
        [AppStringLength(VM.UserVM.PWD_MINLEN, DB.AppUser.PWD_LENGTH)]
        public string Password { get; set; }

        [AppRequired]
        public string FullName { get; set; }

        [AppRequired]
        public string Gender { get; set; }

        [AppPhone]
        public string Phone { get; set; }

        [AppRequired]
        public DateTime Birthday { get; set; }

        [AppRequired]
        [AppEmail]
        public string Email { get; set; }
        public int? AppRoleId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
