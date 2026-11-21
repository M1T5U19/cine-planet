
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

static string EncontrarDiretorio(string nome)
{
    var diretorio = new DirectoryInfo(Directory.GetCurrentDirectory());

    while (diretorio is not null)
    {
        var candidato = Path.Combine(diretorio.FullName, nome);
        if (Directory.Exists(candidato))
            return candidato;
        diretorio = diretorio.Parent;
    }

    throw new DirectoryNotFoundException($"Não foi possível localizar o diretório '{nome}'.");
}

var webRoot = EncontrarDiretorio("cine-planet-site");
var dashboardRoot = EncontrarDiretorio("dashboard");

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