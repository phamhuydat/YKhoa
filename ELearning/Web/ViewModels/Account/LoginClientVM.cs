using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Account
{
    public class LoginClientVM
    {
        [Required]
        public string MSSV { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Ghi nhớ mật khẩu")]
        public bool RememberMe { get; set; }

    }
}
