using Mapster;
using VerticalSliceArchitectureTemplate.Common.EfCore;
using VerticalSliceArchitectureTemplate.Domain;
using VerticalSliceArchitectureTemplate.Features.Games.Common;

namespace VerticalSliceArchitectureTemplate.Features.Games;

internal sealed class PlayTurnCommand(AppDbContext db)
    : IEndpoint,
        IRequestHandler<PlayTurnCommand.Request, PlayTurnCommand.Response>
{
    internal sealed record Request(GameId GameId, int Row, int Column, Tile Player)
        : IRequest<Response>;

    internal sealed record Response(GameViewModel? Game);

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapCommandPost<Request, Response>("/games/play") //{GameId}/
            .WithTags("Games")
            .WithDescription("Plays a turn")
            .AllowAnonymous();
    }

    public async ValueTask<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var game = await db.FindAsync<Game>(request.GameId);

        if (game is null)
        {
            return new Response(null);
        }

        game.MakeMove(request.Row, request.Column, request.Player);
        await db.SaveChangesAsync(cancellationToken);

        var output = game.Adapt<GameViewModel>();
        return new Response(output);
    }
}
