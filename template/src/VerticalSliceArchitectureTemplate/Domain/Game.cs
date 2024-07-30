using Ardalis.GuardClauses;

namespace VerticalSliceArchitectureTemplate.Domain;

[ValueObject<Guid>]
public readonly partial record struct GameId;

public class Game
{
    public GameId Id { get; init; } = GameId.From(Guid.NewGuid());

    public const int MaxNameLength = 50;
    public string Name { get; set; }
    public GameState State { get; private set; } = GameState.XTurn;

    public Board Board { get; private set; } = null!;

    private const int BoardSize = 3;

    // EF Core constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Game() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Game(GameId id, string name)
    {
        Guard.Against.StringTooLong(name, MaxNameLength);

        Id = id;
        Name = name;

        Reset();
    }

    public void MakeMove(int row, int column, Tile tile)
    {
        if (State is GameState.XWon or GameState.OWon)
        {
            throw new InvalidOperationException("Game is already over");
        }

        if (tile != Tile.X && tile != Tile.O)
        {
            throw new InvalidOperationException("Invalid tile");
        }
        if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
        {
            throw new InvalidOperationException("Invalid position");
        }
        if (Board.Value[row][column] != Tile.Empty)
        {
            throw new InvalidOperationException("Position is already taken");
        }

        State = State switch
        {
            GameState.XTurn when tile == Tile.X => GameState.OTurn,
            GameState.OTurn when tile == Tile.O => GameState.XTurn,
            _ => throw new InvalidOperationException("Game is already over")
        };

        Board.Value[row][column] = tile;

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

        for (var i = 0; i < BoardSize; i++)
        {
            if (
                Board.Value[i][0] != Tile.Empty
                && Board.Value[i][0] == Board.Value[i][1]
                && Board.Value[i][0] == Board.Value[i][2]
            )
            {
                winner = Board.Value[i][0];
                return true;
            }
            if (
                Board.Value[0][i] != Tile.Empty
                && Board.Value[0][i] == Board.Value[1][i]
                && Board.Value[0][i] == Board.Value[2][i]
            )
            {
                winner = Board.Value[0][i];
                return true;
            }
        }

        if (
            Board.Value[0][0] != Tile.Empty
            && Board.Value[0][0] == Board.Value[1][1]
            && Board.Value[0][0] == Board.Value[2][2]
        )
        {
            winner = Board.Value[0][0];
            return true;
        }

        if (
            Board.Value[0][2] != Tile.Empty
            && Board.Value[0][2] == Board.Value[1][1]
            && Board.Value[0][2] == Board.Value[2][0]
        )
        {
            winner = Board.Value[0][2];
            return true;
        }

        return Board.Value.SelectMany(row => row).All(tile => tile != Tile.Empty);
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
