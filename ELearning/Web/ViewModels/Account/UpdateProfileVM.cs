using Share.Consts;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Web.ViewModels.Account
{
    public class UpdateProfileVM
    {

        public int Id { get; set; }
        public string Username { get; set; }
        [MaxLength(DB.AppUser.FULLNAME_LENGTH)]
        public string fullname { get; set; }
        [MaxLength(DB.AppUser.PHONE_LENGTH)]
        public string phone { get; set; }
        [MaxLength(DB.AppUser.EMAIL_LENGTH)]
        public string email { get; set; }
        [MaxLength(DB.AppUser.ADDRESS_LENGTH)]
        public string address { get; set; }

        public IFormFile Avatar { get; set; }
        public string? AvatarPath { get; set; }
    }
}
