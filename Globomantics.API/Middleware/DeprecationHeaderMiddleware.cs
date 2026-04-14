namespace Globomantics.API.Middleware
{
    public class DeprecationHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly Dictionary<string, DeprecationInfo> DeprecatedVersions = new()
        {
            ["v1"] = new(
                SunsetDate: "Sat, 31 Dec 2027 23:59:59 GMT",
                MigrationGuideUrl: "https://api.example.com/docs/migration/v1-to-v2"
            )
        };

        public DeprecationHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var version = ExtractVersion(context);

            if (version is not null && DeprecatedVersions.TryGetValue(version, out var info))
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers["Deprecation"] = "true";
                    context.Response.Headers["Sunset"] = info.SunsetDate;
                    context.Response.Headers.Append("Link",
                        $"<{info.MigrationGuideUrl}>; rel=\"deprecation\"");
                    return Task.CompletedTask;
                });
            }

            await _next(context);
        }

        private static string? ExtractVersion(HttpContext context)
        {
            return context.Request.Query.TryGetValue("api-version", out var v)
                ? v.ToString()
                : null;
        }

        private record DeprecationInfo(string SunsetDate, string MigrationGuideUrl);
    }

}
