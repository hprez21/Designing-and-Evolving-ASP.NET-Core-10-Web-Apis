using Globomantics.API.Middleware;
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
});

builder.Services.AddOpenApi("v2", options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    options.ShouldInclude = (description) =>
    description.GroupName == null || description.GroupName == "v2";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseMiddleware<DeprecationHeaderMiddleware>();

app.MapControllers();

app.Run();
