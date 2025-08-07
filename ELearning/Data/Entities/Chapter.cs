using Data.Entities.Base;

namespace Data.Entities
{
    public class Chapter : AppEntityBase
    {
        public Chapter()
        {
            questions = new HashSet<Question>();
            autoExam = new HashSet<AutomaticExam>();
        }
        public int SubjectId { get; set; }
        public string ChapterName { get; set; }
        //fk
        public Subject Subject { get; set; }
        public virtual ICollection<Question> questions { get; set; }
        public virtual ICollection<AutomaticExam> autoExam { get; set; }
    }
}