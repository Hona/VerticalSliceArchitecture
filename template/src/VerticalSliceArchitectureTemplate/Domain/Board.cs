namespace VerticalSliceArchitectureTemplate.Domain;

public class Board
{
    public required Tile[][] Value { get; init; }

    public Tile GetTileAt(BoardPosition position) => Value[position.Row][position.Column];

    public void SetTileAt(BoardPosition position, Tile tile) =>
        Value[position.Row][position.Column] = tile;
}
