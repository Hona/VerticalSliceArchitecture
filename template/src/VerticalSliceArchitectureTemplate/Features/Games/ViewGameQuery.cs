using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureTemplate.Features.Games.Common;

namespace VerticalSliceArchitectureTemplate.Features.Games;

public sealed record ViewGameRequest(GameId GameId);

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
        var response = await db
            .Games.AsNoTracking()
            .Where(x => x.Id == request.GameId)
            .ProjectToResponse()
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            await SendResultAsync(TypedResults.NotFound());
            return;
        }

        await SendResultAsync(TypedResults.Ok(response));
    }
}
