using Data.Entities.Base;

namespace Data.Entities
{
    public class Question : AppEntityBase
    {
        public Question()
        {
            answers = new HashSet<Answer>();
            ExamDetails = new HashSet<ExamDetails>();
            ResultDetails = new HashSet<ResultDetails>();
        }
        public string Content { get; set; }
        public int Level { get; set; }
        public bool Status { get; set; }
        public int SubjectId { get; set; }
        public int ChapterId {  get; set; }
        //fk
        public Chapter chapter { get; set; }
        public Subject subject { get; set; }
        public ICollection<Answer> answers { get; set; }
        public ICollection<ExamDetails> ExamDetails { get; set; }
        public ICollection<ResultDetails> ResultDetails { get; set; }
    }
}