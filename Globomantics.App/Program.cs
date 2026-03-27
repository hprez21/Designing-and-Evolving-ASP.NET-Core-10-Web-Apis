using Spectre.Console;
using System.Net.Http.Json;


const string baseUrl = "http://localhost:5038";
const string productId = "b2c3d4e5-0002-0002-0002-000000000001";

using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

AnsiConsole.Write(
    new FigletText("Globomantics")
        .Centered()
        .Color(Color.Blue));

AnsiConsole.Write(new Rule("[dim]Product Detail -- Before additive changes vs After additive changes[/]").Centered());
AnsiConsole.WriteLine();

// 1. Connectivity check
bool reachable = false;
await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("grey"))
    .StartAsync("Connecting to API...", async ctx =>
    {
        try
        {
            using var ping = await client.GetAsync("/products?api-version=v1");
            reachable = true;
        }
        catch (HttpRequestException)
        {
            reachable = false;
        }
    });

if (!reachable)
{
    AnsiConsole.Write(
        new Panel(
            "[red bold]Could not connect to the API.[/]\n\n" +
            "Make sure to start the [bold yellow]Globomantics.API[/] project first " +
            $"and that it is listening on [underline]{baseUrl}[/].")
        .Header("[red] Connection Error [/]")
        .BorderStyle(Style.Parse("red"))
        .Padding(1, 1));
    return;
}

// 2. Version selection
var version = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Which model version do you want to query?")
        .AddChoices(
            "Before additive changes",
            "After adding rating, reviews and tags",
            "V1 \u2014 Inspect deprecation headers"));

AnsiConsole.WriteLine();

try
{
    if (version.StartsWith("Before"))
        await FetchAndDisplayV1(client, productId);
    else if (version.StartsWith("After"))
        await FetchAndDisplayV2(client, productId);
    else
        await FetchAndDisplayV1WithDeprecationCheck(client, productId);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
}

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine("[dim]Press any key to exit...[/]");
Console.ReadKey(intercept: true);

static async Task FetchAndDisplayV1(HttpClient client, string productId)
{
    AnsiConsole.Write(new Rule("[bold blue] Version 1 [/]").RuleStyle("blue dim"));

    ProductResponseBase? product = null;
    HttpResponseMessage? response = null;
    Exception? fetchError = null;

    await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("blue"))
        .StartAsync("Fetching V1...", async ctx =>
        {
            try
            {
                response = await client.GetAsync($"/products/{productId}?api-version=1.0");
                if (response.IsSuccessStatusCode)
                    product = await response.Content.ReadFromJsonAsync<ProductResponseBase>();
            }
            catch (Exception ex)
            {
                fetchError = ex;
            }
        });

    if (fetchError is not null)
    {
        AnsiConsole.WriteException(fetchError, ExceptionFormats.ShortenPaths);
        return;
    }

    if (response is null || !response.IsSuccessStatusCode || product is null)
    {
        AnsiConsole.MarkupLine($"[red bold]X {(int?)response?.StatusCode} {response?.ReasonPhrase ?? "No response"}[/]");
        return;
    }

    try
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .AddColumn(new TableColumn("[bold blue]Field[/]").Width(18))
            .AddColumn(new TableColumn("[bold]Value[/]"));

        table.AddRow("[dim]Id[/]", $"[grey]{product.Id}[/]");
        table.AddRow("[dim]Name[/]", $"[white bold]{Markup.Escape(product.Name)}[/]");
        table.AddRow("[dim]Description[/]", Markup.Escape(product.Description ?? "-"));
        table.AddRow("[dim]Price[/]", $"[green bold]${product.Price:F2}[/]");
        table.AddRow("[dim]Category Id[/]", $"[grey]{product.CategoryId}[/]");
        table.AddRow("[dim]Category Name[/]", $"[cyan]{Markup.Escape(product.CategoryName ?? "-")}[/]");
        table.AddRow("[dim]Created At[/]", $"[grey]{product.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC[/]");

        AnsiConsole.Write(table);
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
    }
}

static async Task FetchAndDisplayV2(HttpClient client, string productId)
{
    AnsiConsole.Write(new Rule("[bold yellow] Version 2 [/]").RuleStyle("yellow dim"));

    ProductResponseEnriched? product = null;
    HttpResponseMessage? response = null;
    Exception? fetchError = null;

    await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("yellow"))
        .StartAsync("Fetching V2...", async ctx =>
        {
            try
            {
                response = await client.GetAsync($"/products/{productId}?api-version=2.0");
                if (response.IsSuccessStatusCode)
                    product = await response.Content.ReadFromJsonAsync<ProductResponseEnriched>();
            }
            catch (Exception ex)
            {
                fetchError = ex;
            }
        });

    if (fetchError is not null)
    {
        AnsiConsole.WriteException(fetchError, ExceptionFormats.ShortenPaths);
        return;
    }

    if (response is null || !response.IsSuccessStatusCode || product is null)
    {
        AnsiConsole.MarkupLine($"[red bold]X {(int?)response?.StatusCode} {response?.ReasonPhrase ?? "No response"}[/]");
        return;
    }

    try
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("yellow"))
            .AddColumn(new TableColumn("[bold yellow]Field[/]").Width(18))
            .AddColumn(new TableColumn("[bold]Value[/]"));

        table.AddRow("[dim]Id[/]", $"[grey]{product.Id}[/]");
        table.AddRow("[dim]Name[/]", $"[white bold]{Markup.Escape(product.Name)}[/]");
        table.AddRow("[dim]Description[/]", Markup.Escape(product.Description ?? "-"));
        table.AddRow("[dim]Price[/]", $"[green bold]${product.Price:F2}[/]");
        table.AddRow("[dim]Category Id[/]", $"[grey]{product.CategoryId}[/]");
        table.AddRow("[dim]Category Name[/]", $"[cyan]{Markup.Escape(product.CategoryName ?? "-")}[/]");
        table.AddRow("[dim]Created At[/]", $"[grey]{product.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC[/]");
        table.AddRow("[yellow]Average Rating[/]", product.AverageRating is not null
            ? $"[yellow bold]{product.AverageRating:F1} star[/]"
            : "[dim]N/A[/]");
        table.AddRow("[yellow]Review Count[/]", product.ReviewCount is not null
            ? $"[yellow]{product.ReviewCount}[/]"
            : "[dim]N/A[/]");
        table.AddRow("[yellow]Tags[/]", product.Tags is { Count: > 0 }
            ? string.Join("  ", product.Tags.Select(t => $"[on blue] {Markup.Escape(t)} [/]"))
            : "[dim]none[/]");

        AnsiConsole.Write(table);
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
    }
}

#region Deprecation-Aware Fetch

static async Task FetchAndDisplayV1WithDeprecationCheck(HttpClient client, string productId)
{
    AnsiConsole.Write(new Rule("[bold blue] V1 \u2014 Deprecation Header Inspection [/]").RuleStyle("blue dim"));

    ProductResponseBase? product = null;
    HttpResponseMessage? response = null;
    Exception? fetchError = null;

    await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("blue"))
        .StartAsync("Fetching V1 (checking deprecation headers)...", async ctx =>
        {
            try
            {
                response = await client.GetAsync($"/products/{productId}?api-version=v1");
                if (response.IsSuccessStatusCode)
                    product = await response.Content.ReadFromJsonAsync<ProductResponseBase>();
            }
            catch (Exception ex)
            {
                fetchError = ex;
            }
        });

    if (fetchError is not null)
    {
        AnsiConsole.WriteException(fetchError, ExceptionFormats.ShortenPaths);
        return;
    }

    if (response is null || !response.IsSuccessStatusCode || product is null)
    {
        AnsiConsole.MarkupLine($"[red bold]X {(int?)response?.StatusCode} {response?.ReasonPhrase ?? "No response"}[/]");
        return;
    }

    // -- Deprecation header inspection -----------------------------------------
    bool isDeprecated = response.Headers.TryGetValues("Deprecation", out var deprecationValues)
        && deprecationValues.Any(v => v.Equals("true", StringComparison.OrdinalIgnoreCase));

    if (isDeprecated)
    {
        string sunset = response.Headers.TryGetValues("Sunset", out var sunsetValues)
            ? sunsetValues.FirstOrDefault() ?? "N/A"
            : "N/A";

        string migrationLink = "N/A";
        if (response.Headers.TryGetValues("Link", out var linkValues))
        {
            var linkHeader = linkValues.FirstOrDefault() ?? string.Empty;
            var match = System.Text.RegularExpressions.Regex.Match(
                linkHeader, @"<([^>]+)>;\s*rel=\""deprecation\""");
            if (match.Success)
                migrationLink = match.Groups[1].Value;
        }

        AnsiConsole.Write(
            new Panel(
                $"[bold]This API version is marked as deprecated.[/]\n\n" +
                $"  [dim]Deprecation:[/]     [yellow]true[/]\n" +
                $"  [dim]Sunset:[/]          [red]{Markup.Escape(sunset)}[/]\n" +
                $"  [dim]Migration guide:[/] [underline blue]{Markup.Escape(migrationLink)}[/]")
            .Header("[yellow bold] \u26a0 Deprecation Notice [/]")
            .BorderStyle(Style.Parse("yellow"))
            .Padding(1, 1));

        AnsiConsole.WriteLine();
    }
    else
    {
        AnsiConsole.MarkupLine("[dim]No deprecation headers found in this response.[/]");
        AnsiConsole.WriteLine();
    }

    // -- Product data ----------------------------------------------------------
    try
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .AddColumn(new TableColumn("[bold blue]Field[/]").Width(18))
            .AddColumn(new TableColumn("[bold]Value[/]"));

        table.AddRow("[dim]Id[/]", $"[grey]{product.Id}[/]");
        table.AddRow("[dim]Name[/]", $"[white bold]{Markup.Escape(product.Name)}[/]");
        table.AddRow("[dim]Description[/]", Markup.Escape(product.Description ?? "-"));
        table.AddRow("[dim]Price[/]", $"[green bold]${product.Price:F2}[/]");
        table.AddRow("[dim]Category Id[/]", $"[grey]{product.CategoryId}[/]");
        table.AddRow("[dim]Category Name[/]", $"[cyan]{Markup.Escape(product.CategoryName ?? "-")}[/]");
        table.AddRow("[dim]Created At[/]", $"[grey]{product.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC[/]");

        AnsiConsole.Write(table);
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
    }
}

#endregion

record ProductResponseBase(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    string? CategoryName,
    DateTime CreatedAt);

record ProductResponseEnriched(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    string? CategoryName,
    DateTime CreatedAt,
    double? AverageRating,
    int? ReviewCount,
    List<string>? Tags);
