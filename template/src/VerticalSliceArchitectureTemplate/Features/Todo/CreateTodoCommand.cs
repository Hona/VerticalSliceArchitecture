namespace VerticalSliceArchitectureTemplate.Features.Games;

internal sealed class CreateTodoCommand(HttpClient client)
    : IEndpoint,
        IRequestHandler<CreateTodoCommand.Request, CreateTodoCommand.Response>
{
    internal sealed record Request(string Input) : IRequest<Response>;

    internal sealed record Response(string Output);

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapCommandPost<Request, Response>("/todos")
            .WithTags("Todos")
            .WithDescription("Create a new todo, somehow.")
            .AllowAnonymous();
    }

    public async ValueTask<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var randomData = await client.GetStringAsync(
            "https://jsonplaceholder.typicode.com/todos/1",
            cancellationToken
        );

        return new Response(randomData);
    }
}
