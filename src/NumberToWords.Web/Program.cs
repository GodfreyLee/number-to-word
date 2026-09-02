using NumberToWords.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/convert", (string? amount) =>
{
    if (string.IsNullOrWhiteSpace(amount))
    {
        return Results.BadRequest(new { error = "amount is required." });
    }

    try
    {
        var result = NumberToWordsConverter.Convert(amount);
        return Results.Ok(new { words = result.Words, note = result.Note });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
