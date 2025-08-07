using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class ExamDetailsConfig : IEntityTypeConfiguration<ExamDetails>
    {
        public void Configure(EntityTypeBuilder<ExamDetails> builder)
        {
            builder.HasKey(m => new { m.ExamId, m.QuestionId });

            builder.HasOne(x => x.Question)
                .WithMany(x => x.ExamDetails)
                .HasForeignKey(x => x.QuestionId);
        }
    }
}