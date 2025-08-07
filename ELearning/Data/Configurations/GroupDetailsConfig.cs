using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class GroupDetailsConfig : IEntityTypeConfiguration<GroupDetails>
    {
        public void Configure(EntityTypeBuilder<GroupDetails> builder)
        {
            builder.HasKey(x => new { x.Id, x.GroupId, x.UserId });
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.Group)
                .WithMany(x => x.GroupDetails)
                .HasForeignKey(x => x.GroupId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.GroupDetails)
                .HasForeignKey(x => x.UserId);
        }
    }
}