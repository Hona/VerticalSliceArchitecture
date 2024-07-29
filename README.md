# VerticalSliceArchitecture
Spend less time over-engineering, and more time coding. The template has a focus on convenience, and developer confidence.

> [!IMPORTANT]
> This template is undergoing a rebuild, ready for version 2! 🥳 See my experimental version 1 template [here](https://github.com/SSWConsulting/SSW.VerticalSliceArchitecture)
>
> Please wait patiently as this reaches the stable version, there's many important things to finish.
>
> **Please ⭐ the repository to show your support!**
>
> If you would like updates, feel free to 'Watch' the repo, that way you'll see the release in your GitHub home feed.

As mentioned everything is WIP, here's a very quick list of the features:

- A compelling example with the TikTacToe game! 🎮
- Rich Domain:
    - with Vogen for Value-Objects
    - with FluentResults for errors as values instead of exceptions
    - For the Domain, start with an anemic Domain, then as use cases reuse logic, refactor into this more explicit Domain
- Feature Slices:
    - Use cases follow CQRS using Mediator (source gen alternative of MediatR)
    - REPR pattern for the use cases
    - 1 File per use case, containing the endpoint mapping, request, response, handler & application logic
        - Endpoint is source generated
    - For use cases, start with 'just get it working' style code, then refactor into the Domain/Common code.
    - Mapster for source gen/explicit mapping, for example from Domain -> Response/ViewModels
- Common:
    - EF Core, with fluent configuration
    - This sample shows simple config to map a rich entity to EF Core without needing a data model (choose how you'd do this for your project)
- Architecture Tests
    - Pre configured VSA architecture tests, using NuGet (Hona.ArchitectureTests). The template has configured which parts of the codebase relate to which VSA concepts. 🚀
- TODO:
    - Add Mediator pipelines for cross cutting concerns on use cases, like logging, auth, validation (FluentValidation) etc 
    - Unit Test domain
    - Test Containers, etc for integration testing the use cases

To demostrate the template, here is a current whole vertical slice/use case!

```cs
// 👇🏻 Vogen for Strong IDs + 👇🏻 'GameId' field is hydrated from the route parameter
internal sealed record PlayTurnRequest(GameId GameId, int Row, int Column, Tile Player);

// 👇🏻 TypedResults for write once output as well as Swagger documentation
internal sealed class PlayTurnCommand(AppDbContext db)
    : Endpoint<PlayTurnRequest, Results<Ok<GameResponse>, NotFound>>
{
    // 👇🏻 FastEndpoints for a super easy Web API
    public override void Configure()
    {
        Post("/games/{GameId}/play-turn");
        Summary(x =>
        {
            x.Description = "Make a move in the game";
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        PlayTurnRequest request,
        CancellationToken cancellationToken
    )
    {
        // 👇🏻 EF Core without crazy abstractions over the abstraction
        var game = await db.FindAsync<Game>(request.GameId);

        if (game is null)
        {
            await SendResultAsync(TypedResults.NotFound());
            return;
        }

        // 👇🏻 Rich Domain for high value/shared logic
        game.MakeMove(request.Row, request.Column, request.Player);
        await db.SaveChangesAsync(cancellationToken);

        // 👇🏻 Mapster to easily get a view model
        var output = game.Adapt<GameResponse>();
        await SendResultAsync(TypedResults.Ok(output));
    }
}
```