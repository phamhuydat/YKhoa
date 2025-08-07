using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class ResultConfig : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> builder)
        {
            builder.HasKey(x => new { x.Id, x.ExamId, x.UserId });
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}