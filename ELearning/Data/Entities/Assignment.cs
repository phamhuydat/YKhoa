using Data.Entities.Base;

namespace Data.Entities
{
    public class Assignment : AppEntityBase
    {
        public int SubjectId { get; set; }
        public int UserId { get; set; }
        //fk
        public Subject Subject { get; set; }
        public Users Users { get; set; }
    }
}