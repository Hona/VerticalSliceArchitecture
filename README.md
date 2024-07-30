[![](docs/banner-tall.png)](https://github.com/Hona/VerticalSliceArchitecture)
[![](https://img.shields.io/github/actions/workflow/status/Hona/VerticalSliceArchitecture/dotnet.yml?branch=main)](https://github.com/Hona/VerticalSliceArchitecture/actions?query=branch%3Amain)
[![](https://img.shields.io/github/release/Hona/VerticalSliceArchitecture.svg?label=latest%20release&color=007edf)](https://github.com/Hona/VerticalSliceArchitecture/releases/latest)
[![](https://img.shields.io/nuget/dt/Hona.VerticalSliceArchitecture.Template.svg?label=downloads&color=007edf&logo=nuget)](https://www.nuget.org/packages/Hona.VerticalSliceArchitecture.Template)
[![](https://img.shields.io/librariesio/dependents/nuget/Hona.VerticalSliceArchitecture.Template.svg?label=dependent%20libraries)](https://libraries.io/nuget/Hona.VerticalSliceArchitecture.Template)
[![GitHub Repo stars](https://img.shields.io/github/stars/Hona/VerticalSliceArchitecture)](https://github.com/Hona/VerticalSliceArchitecture/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/Hona/VerticalSliceArchitecture)](https://github.com/Hona/VerticalSliceArchitecture/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/Hona/VerticalSliceArchitecture)](https://github.com/Hona/VerticalSliceArchitecture)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/Hona/VerticalSliceArchitecture)](https://github.com/Hona/VerticalSliceArchitecture/graphs/commit-activity)
[![open issues](https://img.shields.io/github/issues/Hona/VerticalSliceArchitecture)](https://github.com/Hona/VerticalSliceArchitecture/issues)

Spend less time over-engineering, and more time coding. The template has a focus on convenience, and developer confidence.

Want to see what a vertical slice looks like? [Jump to the code snippet!](#full-code-snippet)

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-primary.png?raw=true" />
</p>

> [!IMPORTANT]
> This template is undergoing a rebuild, ready for version 2! 🥳 See my experimental version 1 template [here](https://github.com/SSWConsulting/SSW.VerticalSliceArchitecture)
>
> Please wait patiently as this reaches the stable version, there's many important things to finish.
>
> **Please ⭐ the repository to show your support!**
>
> If you would like updates, feel free to 'Watch' the repo, that way you'll see the release in your GitHub home feed.

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-primary.png?raw=true" />
</p>

<h3 align="center"><strong>Getting started ⚡</strong></h3>

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-primary.png?raw=true" />
</p>

#### dotnet CLI

To install the template from NuGet.org run the following command:

```bash
dotnet new install Hona.VerticalSliceArchitecture.Template
```

Then create a new solution:

```bash
mkdir Sprout
cd Sprout

dotnet new hona-vsa
```

Finally, to update the template to the latest version run:

```bash
dotnet new update
```

#### GUI

```bash
dotnet new install Hona.VerticalSliceArchitecture.Template
```

then create:

<img src="https://github.com/user-attachments/assets/797575c2-1304-4501-b9d2-6b6863ace0f3" width="500" />


<h3 align="center"><strong>Features ✨</strong></h3>

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-primary.png?raw=true" />
</p>

### A compelling example with the TikTacToe game! 🎮

```cs
var game = new Game(...);
game.MakeMove(0, 0, Tile.X);
game.MakeMove(0, 1, Tile.Y);
```

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-secondary.png?raw=true" />
</p>

### Rich Domain (thank you DDD!)

- with Vogen for Value-Objects
- with FluentResults for errors as values instead of exceptions
- For the Domain, start with an anemic Domain, then as use cases reuse logic, refactor into this more explicit Domain

```cs
[ValueObject<Guid>]
public readonly partial record struct GameId;

public class Game
{
    public GameId Id { get; init; } = GameId.From(Guid.NewGuid());

    ...
```

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-secondary.png?raw=true" />
</p>

### Quick to write feature slices

- Use cases follow CQRS using Mediator (source gen alternative of MediatR)
- REPR pattern for the use cases
- 1 File per use case, containing the endpoint mapping, request, response, handler & application logic
    - Endpoint is source generated
- For use cases, start with 'just get it working' style code, then refactor into the Domain/Common code.
- Mapster for source gen/explicit mapping, for example from Domain -> Response/ViewModels

`Features/MyThings/MyQuery.cs`

```cs
internal sealed record MyRequest(string Text);
internal sealed record MyResponse(int Result);

internal sealed class MyQuery(AppDbContext db)
    : Endpoint<MyRequest, Results<Ok<GameResponse>, BadRequest>>
{
    public override void Configure()
    {
        Get("/my/{Text}");
    }

    public override async Task HandleAsync(
        MyRequest request,
        CancellationToken cancellationToken
    )
    {
        var thing = await db.Things.SingleAsync(x => x.Text == Text, cancellationToken);

        if (thing is null)
        {
            await SendResultAsync(TypedResults.BadRequest());
            return;
        }

        var output = new MyResponse(thing.Value);
        await SendResultAsync(TypedResults.Ok(output));
    }
}
```

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-secondary.png?raw=true" />
</p>

### EF Core

- Common:
    - EF Core, with fluent configuration
    - This sample shows simple config to map a rich entity to EF Core without needing a data model (choose how you'd do this for your project)

`Common/EfCore/AppDbContext.cs`

```cs
public class AppDbContext : DbContext
{
    public DbSet<MyEntity> MyEntities { get; set; } = default!;

    ...
}
```

`Common/EfCore/Configuration/MyEntityConfiguration.cs`

```cs
public class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.HasKey(x => x.Id);

        ...
    }
}
```

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-secondary.png?raw=true" />
</p>

### Architecture Tests via NuGet package

- Pre configured VSA architecture tests, using NuGet (Hona.ArchitectureTests). The template has configured which parts of the codebase relate to which VSA concepts. 🚀

```cs
public class VerticalSliceArchitectureTests
{
    [Fact]
    public void VerticalSliceArchitecture()
    {
        Ensure.VerticalSliceArchitecture(x =>
        {
            x.Domain = new NamespacePart(SampleAppAssembly, ".Domain");
            ...
        }).Assert();
    }
}
```

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-secondary.png?raw=true" />
</p>

### Cross Cutting Concerns

- TODO:
    - Add ~~Mediator~~ FastEndpoints pipelines for cross cutting concerns on use cases, like logging, auth, validation (FluentValidation) etc (i.e. Common scoped to Use Cases)


## Automated Testing

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-tertiary.png?raw=true" />
</p>

### Domain - Unit Tested

Easy unit tests for the Domain layer

```cs
[Fact]
public void Game_MakeMove_BindsTile()
{
    // Arrange
    var game = new Game(GameId.From(Guid.NewGuid()), "Some Game");
    var tile = Tile.X;
    
    // Act
    game.MakeMove(0, 0, tile);
    
    // Assert
    game.Board.Value[0][0].Should().Be(tile);
}
```

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-secondary.png?raw=true" />
</p>

### Application - Integration Tested

Easy integration tests for each Use Case or Command/Query

TODO: Test Containers, etc for integration testing the use cases. How does this tie into FastEndpoints now... :D

```cs
TODO
```

TODO: Section on mapping & how important the usages + used by at compile time is! (AM vs Mapperly)

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-secondary.png?raw=true" />
</p>

### Code - Architecture Tested

The code is already architecture tested for VSA, but this is extensible, using [Hona.ArchitectureTests](https://github.com/Hona/ArchitectureTests)

## Full Code Snippet

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

        // 👇🏻 Mapperly to easily get a view model with Usage chain at compile time
        var output = GameResponse.MapFrom(game);
        await SendResultAsync(TypedResults.Ok(output));
    }
}
```

If you read it this far, why not give it a star? ;) 

<p align="center">
    <img src="https://github.com/Hona/VerticalSliceArchitecture/blob/main/docs/divider-tertiary.png?raw=true" />
</p>


<p align="center">
<img src="https://repobeats.axiom.co/api/embed/5491ee3c19266af4f681dcf016c299dfc2973b5f.svg" alt="Repobeats analytics image" />
</p>

