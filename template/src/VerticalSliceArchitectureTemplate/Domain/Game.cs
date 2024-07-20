using Microsoft.AspNetCore.Identity;

namespace VerticalSliceArchitectureTemplate.Domain;

[ValueObject<Guid>]
public readonly partial record struct GameId;

public class Game
{
    public GameId Id { get; init; } = GameId.From(Guid.NewGuid());
    public GameState State { get; private set; } = GameState.XTurn;

    public IReadOnlyList<IReadOnlyList<Tile>> Board => _board.AsReadOnly();
    private Tile[][] _board = null!;

    private const int BoardSize = 3;

    // EF Core
    private Game(Tile[][] board)
    {
        _board = board;
    }

    public Game(GameId id)
    {
        Id = id;
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
            throw new ArgumentException("Invalid tile");
        }
        if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
        {
            throw new ArgumentException("Invalid position");
        }
        if (_board[row][column] != Tile.Empty)
        {
            throw new ArgumentException("Position is already taken");
        }

        State = State switch
        {
            GameState.XTurn when tile == Tile.X => GameState.OTurn,
            GameState.OTurn when tile == Tile.O => GameState.XTurn,
            _ => throw new ArgumentException("Invalid turn")
        };

        _board[row][column] = tile;

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
                _board[i][0] != Tile.Empty
                && _board[i][0] == _board[i][1]
                && _board[i][0] == _board[i][2]
            )
            {
                winner = _board[i][0];
                return true;
            }
            if (
                _board[0][i] != Tile.Empty
                && _board[0][i] == _board[1][i]
                && _board[0][i] == _board[2][i]
            )
            {
                winner = _board[0][i];
                return true;
            }
        }

        if (
            _board[0][0] != Tile.Empty
            && _board[0][0] == _board[1][1]
            && _board[0][0] == _board[2][2]
        )
        {
            winner = _board[0][0];
            return true;
        }

        if (
            _board[0][2] != Tile.Empty
            && _board[0][2] == _board[1][1]
            && _board[0][2] == _board[2][0]
        )
        {
            winner = _board[0][2];
            return true;
        }

        return _board.SelectMany(row => row).All(tile => tile != Tile.Empty);
    }

    private void Reset()
    {
        _board = new Tile[BoardSize][];

        for (var i = 0; i < BoardSize; i++)
        {
            _board[i] = new Tile[BoardSize];
            for (var j = 0; j < BoardSize; j++)
            {
                _board[i][j] = Tile.Empty;
            }
        }
        State = GameState.XTurn;
    }
}
