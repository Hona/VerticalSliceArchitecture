using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VerticalSliceArchitectureTemplate.Domain;

namespace VerticalSliceArchitectureTemplate.Common.EfCore.Configuration;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.Board, x => x.ToJson());
        builder
            .Property(x => x.Board)
            .HasField("_board")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
