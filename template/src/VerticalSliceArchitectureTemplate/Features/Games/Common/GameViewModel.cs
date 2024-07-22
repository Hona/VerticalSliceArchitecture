using Mapster;
using VerticalSliceArchitectureTemplate.Domain;

namespace VerticalSliceArchitectureTemplate.Features.Games.Common;

[AdaptFrom(typeof(Game))]
public class GameViewModel : IRegister
{
    public char[][]? Board { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Game, GameViewModel>()
            .Map(
                dest => dest.Board,
                src => src.Board.Value.Select(row => row.Select(GetTileChar).ToArray()).ToArray()
            );
    }

    private static char GetTileChar(Tile tile) =>
        tile switch
        {
            Tile.Empty => ' ',
            Tile.X => 'X',
            Tile.O => 'O',
            _ => '?'
        };
}
