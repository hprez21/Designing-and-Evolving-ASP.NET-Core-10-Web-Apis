using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Globomantics.API.Transformers
{
    public sealed class ApiInfoTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            document.Info = new OpenApiInfo
            {
                Title = "Product Catalog API",
                Version = context.DocumentName,
                Description = context.DocumentName switch
                {
                    "v1" => """
                Product Catalog API v1.
                
                ## Deprecation Notice
                Some fields in this version are deprecated. See individual field descriptions.
                Migrate to v2 before the sunset date.
                
                ## Versioning Policy
                - New optional fields may be added at any time (non-breaking).
                - Clients SHOULD ignore unknown fields in responses.
                - See /docs/api-version-policy for full details.
                """,
                    "v2" => """
                Product Catalog API v2.
                
                ## What's New in v2
                - Flat `price` field replaced by structured `pricing` object.
                - All new features target v2 going forward.
                """,
                    _ => "Product Catalog API"
                },
                Contact = new OpenApiContact
                {
                    Name = "Globomantics Team",
                    Email = "support@globomantics.com",
                    Url = new Uri("https://globomantics.com/api-docs")
                }
            };

            return Task.CompletedTask;
        }
    }

}
