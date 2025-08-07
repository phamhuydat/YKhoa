using Data.Entities.Base;

namespace Data.Entities
{
    public class HandOutExam // phát bài kiểm tra
    {
        public int ExamId { get; set; }
        public int GroupId { get; set; }

        //fk
        public Exam exam { get; set; }
        public Group group { get; set; }
    }
}