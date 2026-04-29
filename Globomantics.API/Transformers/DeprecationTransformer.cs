using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace Globomantics.API.Transformers
{
    public sealed class DeprecationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var methodInfo = context.Description.ActionDescriptor
                .EndpointMetadata
                .OfType<ObsoleteAttribute>()
                .FirstOrDefault();

            if (methodInfo is not null)
            {
                operation.Deprecated = true;

                if (!string.IsNullOrEmpty(methodInfo.Message))
                {
                    operation.Description = string.IsNullOrEmpty(operation.Description)
                        ? $"**DEPRECATED**: {methodInfo.Message}"
                        : $"{operation.Description}\n\n**DEPRECATED**: {methodInfo.Message}";
                }

                operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();
                operation.Extensions["x-sunset-date"] = new 
                    JsonNodeExtension(JsonValue.Create("2027-12-31T23:59:59Z")!);

            }

            return Task.CompletedTask;
        }
    }

}
