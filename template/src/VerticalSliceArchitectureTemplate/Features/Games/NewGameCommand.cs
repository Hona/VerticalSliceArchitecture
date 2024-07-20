using VerticalSliceArchitectureTemplate.Common.EfCore;
using VerticalSliceArchitectureTemplate.Domain;

namespace VerticalSliceArchitectureTemplate.Features.Games;

internal sealed class NewGameCommand(AppDbContext db)
    : IEndpoint,
        IRequestHandler<NewGameCommand.Request, NewGameCommand.Response>
{
    internal sealed record Request : IRequest<Response>;

    internal sealed record Response(GameId GameId);

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapCommandPost<Request, Response>("/games")
            .WithTags("Games")
            .WithDescription("Creates a new game")
            .AllowAnonymous();
    }

    public async ValueTask<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var game = new Game(GameId.From(Guid.NewGuid()));
        db.Add(game);
        await db.SaveChangesAsync(cancellationToken);
        return new Response(game.Id);
    }
}
