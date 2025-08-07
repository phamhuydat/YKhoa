using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class ExamConfig : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasMany(x => x.examDetails)
                 .WithOne(x => x.Exam)
                 .HasForeignKey(x => x.ExamId)
                 .OnDelete(DeleteBehavior.NoAction);


            builder.HasMany(x => x.automaticExams)
                 .WithOne(x => x.Exam)
                 .HasForeignKey(x => x.ExamId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.results)
              .WithOne(x => x.exam)
              .HasForeignKey(x => x.ExamId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.handOutExams)
             .WithOne(x => x.exam)
             .HasForeignKey(x => x.ExamId)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}