using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VerticalSliceArchitectureTemplate.Common.EfCore.Configuration;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(game => game.Id);

        builder.Property(x => x.Name).HasMaxLength(Game.MaxNameLength);

        builder.OwnsOne<Board>(
            game => game.Board,
            game =>
            {
                game.ToJson();
                game.Property(board => board.Value)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                        v =>
                            JsonSerializer.Deserialize<Tile[][]>(v, JsonSerializerOptions.Default)
                            ?? Array.Empty<Tile[]>(),
                        new TileArrayComparer()
                    );
            }
        );
    }
}
