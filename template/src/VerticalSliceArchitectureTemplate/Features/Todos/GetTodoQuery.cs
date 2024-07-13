using VerticalSliceArchitectureTemplate.Common;

namespace VerticalSliceArchitectureTemplate.Features.Todos;

public sealed class GetTodoQuery(HttpClient client)
    : IEndpoint,
        IRequestHandler<GetTodoQuery.Request, GetTodoQuery.Response>
{
    public sealed record Request(string Input) : IRequest<Response>;

    public sealed record Response(string Output);

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapQuery<Request, Response>("/todos").WithTags("Todos").AllowAnonymous();
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
