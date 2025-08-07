namespace Data.Entities
{
    public class AutomaticExam
    {
        public int ExamId { get; set; }
        public int ChapterId { get; set; }
        //fk
        public Exam Exam { get; set; }
        public Chapter Chapter { get; set; }
    }
}