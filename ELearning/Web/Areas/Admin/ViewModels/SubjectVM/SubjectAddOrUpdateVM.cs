using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.ViewModels.SubjectVM
{
    public class SubjectAddOrUpdateVM
    {
        public int Id { get; set; }
        [Required]
        public string SubjectCode { get; set; }
        [Required]
        public string SubjectName { get; set; }
        [Required]
        public int Credit { get; set; }
        [Required]
        public int NumTheory { get; set; }
        [Required]
        public int NumPractice { get; set; }
    }
}
