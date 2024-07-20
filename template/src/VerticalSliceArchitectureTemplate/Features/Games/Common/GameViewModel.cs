using VerticalSliceArchitectureTemplate.Domain;

namespace VerticalSliceArchitectureTemplate.Features.Games.Common;

public class GameViewModel
{
    public char[][] Board { get; set; }

    public GameViewModel(Game game)
    {
        Board = game
            .Board.Select(row =>
                row.Select(tile =>
                        tile switch
                        {
                            Tile.Empty => ' ',
                            Tile.X => 'X',
                            Tile.O => 'O',
                            _ => throw new ArgumentOutOfRangeException(nameof(tile))
                        }
                    )
                    .ToArray()
            )
            .ToArray();
    }
}
