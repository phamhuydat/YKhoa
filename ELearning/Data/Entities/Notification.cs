using Data.Entities.Base;

namespace Data.Entities
{
    public class Notification : AppEntityBase
    {
        public Notification()
        {
            NotificationDetailsDetails = new HashSet<NotificationDetails>();
        }
        public string Content { get; set; }
        public string CreateName { get; set; }

        //fk
        public ICollection<NotificationDetails> NotificationDetailsDetails { get; set; }
    }
}