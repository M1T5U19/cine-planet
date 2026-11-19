
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var webRoot = Path.GetFullPath(
    Path.Combine(Directory.GetCurrentDirectory(), "..", "cine-planet-site", "cineplanetSite"));
var dashboardRoot = Path.GetFullPath(
    Path.Combine(Directory.GetCurrentDirectory(), "..", "dashboard"));

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = webRoot,
});
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CinePlanetApp", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddControllers();
var app = builder.Build();

app.UseExceptionHandler();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dashboardRoot),
    RequestPath = "/dashboard",
});
app.UseCors("CinePlanetApp");

app.MapControllers();

app.Run();