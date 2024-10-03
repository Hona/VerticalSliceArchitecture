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
            _ => throw new InvalidMoveException("Game is already over"),
        };

        Board.SetTileAt(position, tile);

        if (IsGameOver(out var winner))
        {
            State = winner switch
            {
                Tile.X => GameState.XWon,
                Tile.O => GameState.OWon,
                null => GameState.Stalemate,
                _ => throw new UnreachableException("Game was won with empty tiles!"),
            };
        }
    }

    private bool IsGameOver(out Tile? winner)
    {
        winner = null;

        var tiles = Board.Value;

        winner = CheckRowsAndColumns() ?? CheckDiagonals();

        return winner is not null || IsStalemate();

        Tile? CheckRowsAndColumns()
        {
            for (var i = 0; i < _defaultBoardSize.Value; i++)
            {
                // Check columns
                if (
                    tiles[0][i] != Tile.Empty
                    && tiles[0][i] == tiles[1][i]
                    && tiles[0][i] == tiles[2][i]
                )
                {
                    return tiles[0][i];
                }

                // Check rows
                if (
                    tiles[i][0] != Tile.Empty
                    && tiles[i][0] == tiles[i][1]
                    && tiles[i][0] == tiles[i][2]
                )
                {
                    return tiles[i][0];
                }
            }

            return null;
        }

        Tile? CheckDiagonals()
        {
            // Check main diagonal
            if (
                tiles[0][0] != Tile.Empty
                && tiles[0][0] == tiles[1][1]
                && tiles[0][0] == tiles[2][2]
            )
            {
                return tiles[0][0];
            }

            // Check other diagonal
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
