using VerticalSliceArchitectureTemplate.Features.Games;
using VerticalSliceArchitectureTemplate.Features.Games.Common;

namespace VerticalSliceArchitectureTemplate.Integration.Tests.Features.Games;

public class NewGameCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateGame_ReturnsGame()
    {
        // Act
        var response = await Client.PostAsJsonAsync(
            "/games",
            new NewGameRequest("Some Game"),
            JsonSerializerOptions
        );

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PlayTurn_UpdatesBoard()
    {
        // Arrange
        var newGameResponse = await Client.PostAsJsonAsync(
            "/games",
            new NewGameRequest("Some Game"),
            JsonSerializerOptions
        );
        var gameIdRaw = newGameResponse.Headers.Location?.ToString().Split('/').Last();

        gameIdRaw.Should().NotBeNullOrEmpty();

        var gameId = GameId.Parse(gameIdRaw);

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/games/{gameId}/play-turn",
            new PlayTurnRequest(gameId, new BoardPosition(0, 2), Tile.X)
        );

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var gameResponse = await response.Content.ReadFromJsonAsync<GameResponse>(
            JsonSerializerOptions
        );

        gameResponse.Should().NotBeNull();
        gameResponse!.Board.Should().NotBeNull();
        gameResponse.Board![0][2].Should().Be('X');
        gameResponse.Name.Should().Be("Some Game");
    }

    [Theory]
    [InlineData(Tile.X)]
    [InlineData(Tile.O)]
    public async Task WinningMove_ReturnsWinningState(Tile winner)
    {
        var loser = winner == Tile.X ? Tile.O : Tile.X;

        // Arrange
        var game = new Game(GameId.FromNewGuid(), "Some Game");

        if (winner is not Tile.X)
        {
            // X makes a useless move
            game.MakeMove(new BoardPosition(2, 2), Tile.X);
        }

        game.MakeMove(new BoardPosition(0, 0), winner);
        game.MakeMove(new BoardPosition(0, 1), loser);
        game.MakeMove(new BoardPosition(1, 0), winner);
        game.MakeMove(new BoardPosition(1, 1), loser);

        await using (var scope = NewScope())
        {
            var db = scope.GetDbContext();

            db.Games.Add(game);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/games/{game.Id}/play-turn",
            new PlayTurnRequest(game.Id, new BoardPosition(2, 0), winner),
            JsonSerializerOptions
        );

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var gameResponse = await response.Content.ReadFromJsonAsync<GameResponse>(
            JsonSerializerOptions
        );
        gameResponse.Should().NotBeNull();
        gameResponse!
            .State.Should()
            .Be(
                winner switch
                {
                    Tile.X => GameState.XWon,
                    Tile.O => GameState.OWon,
                    _ => throw new ArgumentOutOfRangeException(nameof(winner), winner, null),
                }
            );

        // New scope means that we can fetch latest data from the database
        await using (var scope = NewScope())
        {
            var db = scope.GetDbContext();

            var dbGame = await db.Games.FindAsync(game.Id);
            dbGame.Should().NotBeNull();

            var dbGameResponse = dbGame!.ToResponse();
            dbGameResponse.Should().BeEquivalentTo(gameResponse);
        }
    }
}
