using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class ChapterConfig : IEntityTypeConfiguration<Chapter>
    {
        public void Configure(EntityTypeBuilder<Chapter> builder)
        {
            builder.HasKey(x => new { x.Id });
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.Subject)
                .WithMany(x => x.Chapters)
                .HasForeignKey(x => x.SubjectId);

            builder.HasMany(x => x.autoExam)
                .WithOne(x => x.Chapter)
                .HasForeignKey(x => x.ChapterId)
                .HasPrincipalKey(x => x.Id);

        }
    }
}