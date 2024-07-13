using Microsoft.AspNetCore.Mvc;

namespace VerticalSliceArchitectureTemplate.Common;

public static class EndpointMediatorExtensions
{
    public static RouteHandlerBuilder MapQuery<TRequest, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        endpoints
            .MapGet(
                pattern,
                (
                    [AsParameters] TRequest request,
                    CancellationToken cancellationToken,
                    IMediator mediator
                ) => mediator.Send(request, cancellationToken)
            )
            .Produces<TResponse>();

    public static RouteHandlerBuilder MapCommandPost<TRequest>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        endpoints.MapMethods(pattern, [HttpMethods.Post], GetCommandDelegate<TRequest>());

    public static RouteHandlerBuilder MapCommandPost<TRequest, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        MapCommandPost<TRequest>(endpoints, pattern).Produces<TResponse>();

    public static RouteHandlerBuilder MapCommandPut<TRequest>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        endpoints.MapMethods(pattern, [HttpMethods.Put], GetCommandDelegate<TRequest>());

    public static RouteHandlerBuilder MapCommandPut<TRequest, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        MapCommandPut<TRequest>(endpoints, pattern).Produces<TResponse>();

    public static RouteHandlerBuilder MapCommandDelete<TRequest>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        endpoints.MapMethods(pattern, [HttpMethods.Delete], GetCommandDelegate<TRequest>());

    public static RouteHandlerBuilder MapCommandDelete<TRequest, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        MapCommandDelete<TRequest>(endpoints, pattern).Produces<TResponse>();

    public static RouteHandlerBuilder MapCommandPatch<TRequest>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        endpoints.MapMethods(pattern, [HttpMethods.Patch], GetCommandDelegate<TRequest>());

    public static RouteHandlerBuilder MapCommandPatch<TRequest, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern
    )
        where TRequest : IBaseRequest =>
        MapCommandPatch<TRequest>(endpoints, pattern).Produces<TResponse>();

    private static Delegate GetCommandDelegate<TRequest>()
        where TRequest : IBaseRequest =>
        ([FromBody] TRequest request, CancellationToken cancellationToken, IMediator mediator) =>
            mediator.Send(request, cancellationToken);
}
