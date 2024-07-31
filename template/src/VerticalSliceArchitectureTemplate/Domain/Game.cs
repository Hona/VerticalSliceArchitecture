using System.Diagnostics;
using Ardalis.GuardClauses;

namespace VerticalSliceArchitectureTemplate.Domain;

public class Game
{
    private readonly BoardSize _defaultBoardSize = BoardSize.DefaultBoardSize;

    public GameId Id { get; init; } = GameId.FromNewGuid();

    public const int MaxNameLength = 50;
    public string Name { get; init; }
    public GameState State { get; private set; } = GameState.XTurn;

    public Board Board { get; private set; } = null!;

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

    public void MakeMove(BoardPosition position, Tile tile)
    {
        if (State is GameState.XWon or GameState.OWon)
        {
            throw new InvalidMoveException("Game is already over");
        }

        if (tile != Tile.X && tile != Tile.O)
        {
            throw new InvalidMoveException("Invalid tile");
        }

        if (!position.IsWithin(_defaultBoardSize))
        {
            throw new InvalidMoveException($"Invalid {nameof(position)}");
        }

        if (Board.GetTileAt(position) != Tile.Empty)
        {
            throw new InvalidMoveException("Position is already taken");
        }

        State = State switch
        {
            GameState.XTurn when tile == Tile.X => GameState.OTurn,
            GameState.OTurn when tile == Tile.O => GameState.XTurn,
            GameState.OWon
            or GameState.XWon
                => throw new InvalidMoveException("Game is already over"),
            _ => throw new UnreachableException("Invalid game state")
        };

        Board.SetTileAt(position, tile);

        if (IsGameOver(out var winner))
        {
            State = winner switch
            {
                Tile.X => GameState.XTurn,
                Tile.O => GameState.OWon,
                _ => throw new UnreachableException("Game was won with empty tiles!")
            };
        }
    }

    private bool IsGameOver(out Tile? winner)
    {
        winner = null;

        var tiles = Board.Value;

        var firstTile = tiles[0][0];

        winner = CheckRowsAndColumns() ?? CheckDiagonals();

        return winner is not null || IsStalemate();

        Tile? CheckRowsAndColumns()
        {
            for (var i = 0; i < _defaultBoardSize.Value; i++)
            {
                var firstInColumn = tiles[i][0];
                if (
                    firstInColumn != Tile.Empty
                    && firstInColumn == tiles[i][1]
                    && firstInColumn == tiles[i][2]
                )
                {
                    return firstInColumn;
                }

                var firstInRow = tiles[0][i];
                if (
                    firstInRow != Tile.Empty
                    && firstInRow == tiles[1][i]
                    && firstInRow == tiles[2][i]
                )
                {
                    return firstInRow;
                }
            }

            return null;
        }

        Tile? CheckDiagonals()
        {
            if (firstTile != Tile.Empty && firstTile == tiles[1][1] && firstTile == tiles[2][2])
            {
                return firstTile;
            }

            if (
                tiles[0][2] != Tile.Empty
                && tiles[0][2] == tiles[1][1]
                && tiles[0][2] == tiles[2][0]
            )
            {
                return tiles[0][2];
            }

            return null;
        }

        bool IsStalemate() => tiles.SelectMany(row => row).All(tile => tile != Tile.Empty);
    }

    private void Reset()
    {
        Board = Board.NewBoard(_defaultBoardSize);

        State = GameState.XTurn;
    }
}
