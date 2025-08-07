using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class AssignmentConfig : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.HasKey(x => new { x.Id, x.SubjectId, x.UserId });
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.Subject)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.SubjectId);

            builder.HasOne(x => x.Users)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.UserId);
        }
    }
}