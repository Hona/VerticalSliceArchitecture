using Mapster;
using VerticalSliceArchitectureTemplate.Features.Games.Common;

namespace VerticalSliceArchitectureTemplate.Features.Games;

internal sealed record ViewGameRequest(GameId GameId);

internal sealed class ViewGameQuery(AppDbContext db)
    : Endpoint<ViewGameRequest, Results<Ok<GameResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/games/{GameId}");
        Summary(x =>
        {
            x.Description = "Views the board as its string representation";
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        ViewGameRequest request,
        CancellationToken cancellationToken
    )
    {
        var game = await db.FindAsync<Game>(request.GameId);

        if (game is null)
        {
            await SendResultAsync(TypedResults.NotFound());
            return;
        }

        var output = game.Adapt<GameResponse>();

        await SendResultAsync(TypedResults.Ok(output));
    }
}
