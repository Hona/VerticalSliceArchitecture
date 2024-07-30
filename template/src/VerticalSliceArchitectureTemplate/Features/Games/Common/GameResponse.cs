namespace VerticalSliceArchitectureTemplate.Features.Games.Common;

[Mapper]
public partial class GameResponse
{
    public required string Name { get; set; }
    public char[][]? Board { get; set; }

    [MapProperty(nameof(Game.Board), nameof(Board), Use = nameof(MapBoard))]
    public static partial GameResponse MapFrom(Game source);

    [UserMapping(Default = false)]
    private static char[][]? MapBoard(Board? board) =>
        board?.Value.Select(row => row.Select(GetTileChar).ToArray()).ToArray();

    private static char GetTileChar(Tile tile) =>
        tile switch
        {
            Tile.Empty => ' ',
            Tile.X => 'X',
            Tile.O => 'O',
            _ => '?'
        };
}
