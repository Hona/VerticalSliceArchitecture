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
}
