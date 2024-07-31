namespace VerticalSliceArchitectureTemplate.Domain;

public class Board
{
    public required Tile[][] Value { get; init; }

    public Tile GetTileAt(BoardPosition position) => Value[position.Row][position.Column];

    public void SetTileAt(BoardPosition position, Tile tile) =>
        Value[position.Row][position.Column] = tile;

    public static Board NewBoard(BoardSize size)
    {
        var board = new Board { Value = new Tile[size][] };

        for (var i = 0; i < size; i++)
        {
            board.Value[i] = new Tile[size];
            for (var j = 0; j < size; j++)
            {
                board.Value[i][j] = Tile.Empty;
            }
        }

        return board;
    }
}
