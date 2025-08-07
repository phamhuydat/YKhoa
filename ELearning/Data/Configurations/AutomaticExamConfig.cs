using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class AutomaticExamConfig : IEntityTypeConfiguration<AutomaticExam>
    {
        public void Configure(EntityTypeBuilder<AutomaticExam> builder)
        {
            builder.HasKey(x => new { x.ExamId, x.ChapterId });

        }
    }
}