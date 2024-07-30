namespace VerticalSliceArchitectureTemplate.Domain;

public readonly record struct BoardPosition(int Row, int Column)
{
    public bool IsWithin(BoardSize boardSize) =>
        Row >= 0 && Row < boardSize.Value && Column >= 0 && Column < boardSize.Value;
}