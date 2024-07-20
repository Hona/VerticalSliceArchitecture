global using Hona.Endpoints;
global using Hona.Endpoints.Extensions.Mediator;
global using Mediator;
global using Vogen;

// Allow Strong IDs to generate nice OpenAPI schemas
[assembly: VogenDefaults(
    openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod
)]
