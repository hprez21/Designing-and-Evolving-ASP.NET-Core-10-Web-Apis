using Globomantics.API.Middleware;
using Globomantics.API.Transformers;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi("v1", options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    options.ShouldInclude = (description) =>
    description.GroupName == null || description.GroupName == "v1";

    options.AddOperationTransformer<DeprecationTransformer>();
    options.AddDocumentTransformer<ApiInfoTransformer>();
});

builder.Services.AddOpenApi("v2", options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    options.ShouldInclude = (description) =>
    description.GroupName == null || description.GroupName == "v2";

    options.AddOperationTransformer<DeprecationTransformer>();
    options.AddDocumentTransformer<ApiInfoTransformer>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/productsdemo", out var remainingPath))
    {
        var newPath = $"/v1/productsdemo{remainingPath}{context.Request.QueryString}";
        context.Response.Redirect(newPath, permanent: true);
        return;
    }

    await next();
});

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.UseAuthorization();

app.UseMiddleware<DeprecationHeaderMiddleware>();

app.MapControllers();

app.Run();
