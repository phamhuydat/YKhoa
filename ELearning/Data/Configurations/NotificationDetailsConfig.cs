using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class NotificationDetailsConfig : IEntityTypeConfiguration<NotificationDetails>
    {
        public void Configure(EntityTypeBuilder<NotificationDetails> builder)
        {
            builder.HasKey(x => new { x.GroupId, x.NotificationId });

        }
    }
}