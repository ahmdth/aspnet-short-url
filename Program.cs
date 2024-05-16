using Microsoft.EntityFrameworkCore;
using UrlShort.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApiDbContext>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/url", async (UrlDto url, ApiDbContext db, HttpContext ctx) =>
{
    if (!Uri.TryCreate(url.url, UriKind.Absolute, out var input))
    {
        return Results.BadRequest("Invalid url has been provided.");
    }

    var random = new Random();
    const string alphanumric = "ABCDEFGHIJKLMNOPQRSTWYXYZ0123456789@az";
    var code = new string(Enumerable.Repeat(alphanumric, 8).Select(x => x[random.Next(x.Length)]).ToArray());
    var urlManager = new ShortUrl()
    {
        Url = url.url,
        Short = code
    };
    db.Urls.Add(urlManager);
    db.SaveChangesAsync();
    var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{urlManager.Short}";
    return Results.Ok(new { url = result});
});

app.MapFallback(async (ApiDbContext db, HttpContext ctx) =>
{
    var code = ctx.Request.Path.ToUriComponent().Trim('/');
    var url = await db.Urls.FirstOrDefaultAsync(x => x.Short.Trim() == code.Trim());
    if (url == null)
    {
        return Results.BadRequest("invalid short url.");
    }

    return Results.Redirect(url.Url);
});

app.Run();

record UrlDto(string url);