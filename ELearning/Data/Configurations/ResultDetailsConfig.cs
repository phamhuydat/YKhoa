using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
	public class ResultDetailsConfig : IEntityTypeConfiguration<ResultDetails>
	{
		public void Configure(EntityTypeBuilder<ResultDetails> builder)
		{
			builder.HasKey(x => new { x.Id, x.QuestionId, x.ResultId });
			builder.Property(x => x.Id).ValueGeneratedOnAdd();

			builder.HasOne(x => x.Question)
				.WithMany(x => x.ResultDetails)
				.HasForeignKey(x => x.QuestionId);

			builder.HasOne(x => x.Result)
				.WithMany(x => x.ResultDetails)
				.HasForeignKey(x => x.ResultId)
				.HasPrincipalKey(x => x.Id);

			builder.HasOne(x => x.Answer)
				.WithMany(x => x.ResultDetails)
				.HasForeignKey(x => x.AnswerId);
		}
	}
}