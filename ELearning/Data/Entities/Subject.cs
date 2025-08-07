using Data.Entities.Base;

namespace Data.Entities
{
    public class Subject : AppEntityBase
    {
        public Subject()
        {
            Questions = new HashSet<Question>();
            Assignments = new HashSet<Assignment>();
            Groups = new HashSet<Group>();
            Chapters = new HashSet<Chapter>();
        }

        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int Credit { get; set; } // số tín chỉ
        public int NumTheory { get; set; } // số tiết lý thuyết
        public int NumPractice { get; set; } // số tiết thực hành
        public bool Status { get; set; }

        //fk
        public ICollection<Chapter> Chapters { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<Group> Groups { get; set; }

    }
}