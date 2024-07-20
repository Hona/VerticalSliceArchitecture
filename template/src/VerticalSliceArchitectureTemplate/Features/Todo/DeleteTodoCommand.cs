namespace VerticalSliceArchitectureTemplate.Features.Games;

internal sealed class DeleteTodoCommand(HttpClient client)
    : IEndpoint,
        IRequestHandler<DeleteTodoCommand.Request, DeleteTodoCommand.Response>
{
    internal sealed record Request(string Input) : IRequest<Response>;

    internal sealed record Response(string Output);

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCommandDelete<Request>("/todos").WithTags("Todos").AllowAnonymous();
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
