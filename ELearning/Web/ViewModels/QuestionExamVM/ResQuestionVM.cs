using Data.Entities;
using Web.Areas.Admin.ViewModels;

namespace Web.ViewModels.QuestionExamVM
{
    public class ResQuestionVM : ListItemBaseVM
    {
        public ResQuestionVM()
        {
            answers = new List<Answer>();
        }

        public string Content { get; set; }
        public int AnswerId { get; set; }
        public List<Answer> answers { get; set; }
    }
}
