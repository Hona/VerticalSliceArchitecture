using Ardalis.GuardClauses;

namespace VerticalSliceArchitectureTemplate.Domain;

public record struct BoardPosition(int Row, int Column)
{
    public bool IsWithin(int boardSize) =>
        Row >= 0 && Row < boardSize && Column >= 0 && Column < boardSize;
}

[ValueObject<Guid>]
public readonly partial record struct GameId;

public class Game
{
    public GameId Id { get; init; } = GameId.FromNewGuid();

    public const int MaxNameLength = 50;
    public string Name { get; init; }
    public GameState State { get; private set; } = GameState.XTurn;

    public Board Board { get; private set; } = null!;

    private const int BoardSize = 3;

    // EF Core
    private Game() { }

    public Game(GameId id, string name)
    {
        Guard.Against.StringTooLong(name, MaxNameLength);

        Id = id;
        Name = name;

        Reset();
    }

    public void MakeMove(BoardPosition boardPosition, Tile tile)
    {
        if (State is GameState.XWon or GameState.OWon)
        {
            throw new InvalidOperationException("Game is already over");
        }

        if (tile != Tile.X && tile != Tile.O)
        {
            throw new InvalidOperationException("Invalid tile");
        }

        if (!boardPosition.IsWithin(BoardSize))
        {
            throw new InvalidOperationException("Invalid position");
        }
        if (Board.GetTileAt(boardPosition) != Tile.Empty)
        {
            throw new InvalidOperationException("Position is already taken");
        }

        State = State switch
        {
            GameState.XTurn when tile == Tile.X => GameState.OTurn,
            GameState.OTurn when tile == Tile.O => GameState.XTurn,
            _ => throw new InvalidOperationException("Game is already over")
        };

        Board.SetTileAt(boardPosition, tile);

        if (IsGameOver(out var winner))
        {
            State = winner switch
            {
                Tile.X => GameState.XTurn,
                Tile.O => GameState.OWon,
                _ => throw new InvalidOperationException(nameof(winner))
            };
        }
    }

    private bool IsGameOver(out Tile? winner)
    {
        // TODO: Check this GH Copilot logic
        winner = null;

        var tiles = Board.Value;
        Tile firstTile = tiles[0][0];

        for (var i = 0; i < BoardSize; i++)
        {
            Tile firstInColumn = tiles[i][0];
            if (
                firstInColumn != Tile.Empty
                && firstInColumn == tiles[i][1]
                && firstInColumn == tiles[i][2]
            )
            {
                winner = firstInColumn;
                return true;
            }

            Tile firstInRow = tiles[0][i];
            if (
                firstInRow != Tile.Empty
                && firstInRow == tiles[1][i]
                && firstInRow == tiles[2][i]
            )
            {
                winner = firstInRow;
                return true;
            }
        }

        if (
            firstTile != Tile.Empty
            && firstTile == tiles[1][1]
            && firstTile == tiles[2][2]
        )
        {
            winner = firstTile;
            return true;
        }

        if (
            tiles[0][2] != Tile.Empty
            && tiles[0][2] == tiles[1][1]
            && tiles[0][2] == tiles[2][0]
        )
        {
            winner = tiles[0][2];
            return true;
        }

        return tiles.SelectMany(row => row).All(tile => tile != Tile.Empty);
    }

    private void Reset()
    {
        Board = new Board { Value = new Tile[BoardSize][] };

        for (var i = 0; i < BoardSize; i++)
        {
            Board.Value[i] = new Tile[BoardSize];
            for (var j = 0; j < BoardSize; j++)
            {
                Board.Value[i][j] = Tile.Empty;
            }
        }
        State = GameState.XTurn;
    }
}
