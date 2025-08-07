using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.ViewModels.GroupDetailVM
{
    public class GroupDetailAddOrEditVM
    {
        [Required]
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}
