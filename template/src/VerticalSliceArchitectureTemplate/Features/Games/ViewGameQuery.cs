using Mapster;
using VerticalSliceArchitectureTemplate.Common.EfCore;
using VerticalSliceArchitectureTemplate.Domain;
using VerticalSliceArchitectureTemplate.Features.Games.Common;

namespace VerticalSliceArchitectureTemplate.Features.Games;

internal sealed class ViewGameQuery(AppDbContext db)
    : IEndpoint,
        IRequestHandler<ViewGameQuery.Request, ViewGameQuery.Response>
{
    internal sealed record Request(GameId GameId) : IRequest<Response>;

    internal sealed record Response(GameViewModel? Game);

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapQuery<Request, Response>("/games")
            .WithTags("Games")
            .WithDescription("Views the board as its string representation")
            .AllowAnonymous();
    }

    public async ValueTask<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var game = await db.FindAsync<Game>(request.GameId);

        if (game is null)
        {
            return new Response(null);
        }

        var output = game.Adapt<GameViewModel>();
        return new Response(output);
    }
}
