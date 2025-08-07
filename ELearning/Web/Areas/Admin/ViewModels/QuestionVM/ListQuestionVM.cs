using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.ViewModels.QuestionVM
{
    public class ListQuestionVM : ListItemBaseVM
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int Level { get; set; }
        public string SubjectName { get; set; }
        public string ChapterName { get; set; }
        public int SubjectId { get; set; }
        public int ChapterId { get; set; }

        public ICollection<Answer> Answers { get; set; }

    }
}
