namespace VerticalSliceArchitectureTemplate.Features.Games.Common;

public record GameResponse
{
    public required string Name { get; set; }
    public char[][]? Board { get; set; }
}

[Mapper]
public static partial class GameResponseMapper
{
    [MapProperty(nameof(Game.Board), nameof(Board), Use = nameof(MapBoard))]
    public static partial GameResponse ToResponse(this Game source);

    public static partial IQueryable<GameResponse> ProjectToResponse(this IQueryable<Game> q);

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
