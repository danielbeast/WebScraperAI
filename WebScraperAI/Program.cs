using WebScraperAI.Models;
using WebScraperAI.Services;
using WebScraperAI.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IParse, Parse>();
builder.Services.AddScoped<IScrape, Scrape>();
builder.Services.AddScoped<IScrapeAndParseService, ScrapeAndParseService>();
var app = builder.Build();

//API end point for Scrape and Parse
app.MapPost("/scrape-and-parse", async (PromptModel model, IScrapeAndParseService service) =>
{
    await service.Post(model);
    return Results.Json(model);
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
