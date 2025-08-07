using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
	public class AnswerConfig : IEntityTypeConfiguration<Answer>
	{
		public void Configure(EntityTypeBuilder<Answer> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id).ValueGeneratedOnAdd();

			builder.HasOne(x => x.Question)
				.WithMany(x => x.answers)
				.HasForeignKey(x => x.QuestionId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}