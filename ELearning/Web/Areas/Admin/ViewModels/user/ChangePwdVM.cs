using Shared.Attributes;
using System.ComponentModel.DataAnnotations;
using Web.WebConfig;

namespace Web.Areas.Admin.ViewModels.user
{
    public class ChangePwdVM
    {
        [DataType(DataType.Password)]
        [AppRequired]
        public string Pwd { get; set; }

        [DataType(DataType.Password)]
        [AppRequired]
        [AppMinLength(VM.UserVM.PWD_MINLEN)]
        public string NewPwd { get; set; }

        [DataType(DataType.Password)]
        [AppConfirmPwd("NewPwd")]
        public string ConfirmPassword { get; set; }
        public bool LogoutAfterChangePwd { get; set; }
    }
}
