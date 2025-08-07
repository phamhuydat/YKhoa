using Data.Entities.Base;

namespace Data.Entities
{
    public class Exam : AppEntityBase
    {
        public Exam()
        {
            examDetails = new HashSet<ExamDetails>();
            automaticExams = new HashSet<AutomaticExam>();
            results = new HashSet<Result>();
            handOutExams = new HashSet<HandOutExam>();
        }

        public int SubjectId { get; set; }
        public string Title { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int WorkTime { get; set; }       // Time to do the exam
        public bool SeeAnswer { get; set; }
        public bool MixQuestion { get; set; }
        public bool MixAnswer { get; set; }
        public bool SubmitWhenExit { get; set; } // Submit when exit
        public int EQCount { get; set; }        // number of easy questions
        public int MQCount { get; set; }        // number of medium questions
        public int HQCount { get; set; }        // number of hard questions
        public bool Status { get; set; }
        public bool IsAutomatic { get; set; } // là tự động hay không

        public Subject Subject { get; set; }
        public ICollection<ExamDetails> examDetails { get; set; }
        public ICollection<AutomaticExam> automaticExams { get; set; }
        public ICollection<Result> results { get; set; }
        public ICollection<HandOutExam> handOutExams { get; set; }
    }
}