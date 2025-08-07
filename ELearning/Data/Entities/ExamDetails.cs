using Data.Entities.Base;

namespace Data.Entities
{
    public class ExamDetails
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int? DisplayOrder { get; set; }

        public Exam Exam { get; set; }
        public Question Question { get; set; }
    }
}