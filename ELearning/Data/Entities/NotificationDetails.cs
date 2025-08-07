using System.ComponentModel.DataAnnotations;
namespace Data.Entities
{
    public class NotificationDetails
    {
        [Key]
        public int NotificationId { get; set; }
        public int GroupId { get; set; }

        //fk
        public Notification Notification { get; set; }
        public Group Group { get; set; }
    }
}