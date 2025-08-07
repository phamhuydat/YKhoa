using Data.Entities;
using System.ComponentModel.DataAnnotations;


namespace Web.Areas.Admin.ViewModels.QuestionVM
{
    public class QuestionAddOrEditVM : ListItemBaseVM
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public int ChapterId { get; set; }
        [Required]
        public int Level { get; set; }
        public List<AnswerVM.AnswerAddOrEdit> Options { get; set; }
    }
}
