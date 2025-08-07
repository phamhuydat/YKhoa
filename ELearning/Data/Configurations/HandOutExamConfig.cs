using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class HandOutExamConfig : IEntityTypeConfiguration<HandOutExam>
    {
        public void Configure(EntityTypeBuilder<HandOutExam> builder)
        {
            builder.HasKey(x => new { x.GroupId, x.ExamId });

            builder.HasOne(x => x.group)
                .WithMany(x => x.HandOutExams)
                .HasForeignKey(x => x.GroupId);
        }
    }

}