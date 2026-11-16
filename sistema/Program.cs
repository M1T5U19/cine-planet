using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "cineplanetSite")
});
builder.Services.AddProblemDetails();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CinePlanet.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();
var connectionString = builder.Configuration.GetConnectionString("CinePlanet")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:CinePlanet.");
const string columns = "id, titulo, generos, duracao, sinopse, imagem, ano, diretor, classificacao, nota_imdb";

app.UseExceptionHandler();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/filmes", (CancellationToken cancellationToken) =>
{
    var query = $"""
        SELECT {columns}
        FROM filmes AS f
        WHERE f.id = (
            SELECT MIN(id)
            FROM filmes
            WHERE titulo = f.titulo
        )
        ORDER BY f.titulo
        """;
    return BuscarFilmes(query, cancellationToken);
});

app.MapGet("/api/filmes/{id:int}", BuscarFilme);

app.MapPost("/login", async (HttpContext context, CancellationToken cancellationToken) =>
{
    var form = await context.Request.ReadFormAsync(cancellationToken);
    var nomeDigitado = form["nome"].ToString().Trim();
    var nome = new[] { "Mateus", "Pietro" }.FirstOrDefault(administrador =>
        string.Equals(administrador, nomeDigitado, StringComparison.OrdinalIgnoreCase));

    if (nome is null || form["senha"].ToString() != "1234")
    {
        return Results.LocalRedirect("/login.html?erro=1");
    }

    var identidade = new ClaimsIdentity(
        [new Claim(ClaimTypes.Name, nome)],
        CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identidade));

    return Results.LocalRedirect("/admin.html");
});

var adminApi = app.MapGroup("/api/admin").RequireAuthorization();
adminApi.MapGet("/sessao", (ClaimsPrincipal usuario) =>
    Results.Ok(new { nome = usuario.Identity?.Name }));
adminApi.MapGet("/filmes", (CancellationToken cancellationToken) =>
    BuscarFilmes($"SELECT {columns} FROM filmes ORDER BY titulo", cancellationToken));
adminApi.MapPost("/filmes", CriarFilme);
adminApi.MapPut("/filmes/{id:int}", AtualizarFilme);
adminApi.MapDelete("/filmes/{id:int}", ExcluirFilme);

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/login.html");
}).RequireAuthorization();

app.Run();

async Task<List<Filme>> BuscarFilmes(string query, CancellationToken cancellationToken)
{
    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new MySqlCommand(query, connection);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    var filmes = new List<Filme>();
    while (await reader.ReadAsync(cancellationToken))
    {
        filmes.Add(LerFilme(reader));
    }

    return filmes;
}

async Task<IResult> BuscarFilme(int id, CancellationToken cancellationToken)
{
    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new MySqlCommand(
        $"SELECT {columns} FROM filmes WHERE id = @id",
        connection);
    command.Parameters.AddWithValue("@id", id);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    return await reader.ReadAsync(cancellationToken)
        ? Results.Ok(LerFilme(reader))
        : Results.NotFound(new { message = "Não foi possível localizar o filme." });
}

async Task<IResult> CriarFilme(FilmeEntrada filme, CancellationToken cancellationToken)
{
    var erro = ValidarFilme(filme);
    if (erro is not null)
    {
        return Results.BadRequest(new { message = erro });
    }

    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new MySqlCommand(
        """
        INSERT INTO filmes (
            titulo, generos, duracao, sinopse, imagem, ano, diretor, classificacao, nota_imdb
        ) VALUES (
            @titulo, @generos, @duracao, @sinopse, @imagem, @ano, @diretor, @classificacao, @nota
        )
        """,
        connection);
    AdicionarParametros(command, filme);

    try
    {
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    catch (MySqlException exception) when (exception.Number == 1062)
    {
        return Results.Conflict(new { message = "Já existe um filme com esse título." });
    }

    var id = checked((int)command.LastInsertedId);
    return Results.Created($"/api/filmes/{id}", new { id });
}

async Task<IResult> AtualizarFilme(int id, FilmeEntrada filme, CancellationToken cancellationToken)
{
    var erro = ValidarFilme(filme);
    if (erro is not null)
    {
        return Results.BadRequest(new { message = erro });
    }

    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new MySqlCommand(
        """
        UPDATE filmes
        SET titulo = @titulo, generos = @generos, duracao = @duracao,
            sinopse = @sinopse, imagem = @imagem, ano = @ano,
            diretor = @diretor, classificacao = @classificacao, nota_imdb = @nota
        WHERE id = @id
        """,
        connection);
    AdicionarParametros(command, filme);
    command.Parameters.AddWithValue("@id", id);

    try
    {
        if (await command.ExecuteNonQueryAsync(cancellationToken) == 0)
        {
            return Results.NotFound(new { message = "Não foi possível localizar o filme." });
        }
    }
    catch (MySqlException exception) when (exception.Number == 1062)
    {
        return Results.Conflict(new { message = "Já existe um filme com esse título." });
    }

    return Results.Ok(new { message = "Filme atualizado." });
}

async Task<IResult> ExcluirFilme(int id, CancellationToken cancellationToken)
{
    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new MySqlCommand("DELETE FROM filmes WHERE id = @id", connection);
    command.Parameters.AddWithValue("@id", id);

    return await command.ExecuteNonQueryAsync(cancellationToken) == 0
        ? Results.NotFound(new { message = "Não foi possível localizar o filme." })
        : Results.NoContent();
}

static void AdicionarParametros(MySqlCommand command, FilmeEntrada filme)
{
    command.Parameters.AddWithValue("@titulo", filme.Titulo.Trim());
    command.Parameters.AddWithValue("@generos", filme.Generos.Trim());
    command.Parameters.AddWithValue("@duracao", filme.Duracao.Trim());
    command.Parameters.AddWithValue("@sinopse", filme.Sinopse.Trim());
    command.Parameters.AddWithValue("@imagem", filme.Imagem.Trim());
    command.Parameters.AddWithValue("@ano", (object?)filme.Ano ?? DBNull.Value);
    command.Parameters.AddWithValue(
        "@diretor",
        string.IsNullOrWhiteSpace(filme.Diretor) ? DBNull.Value : filme.Diretor.Trim());
    command.Parameters.AddWithValue(
        "@classificacao",
        string.IsNullOrWhiteSpace(filme.Classificacao) ? DBNull.Value : filme.Classificacao.Trim());
    command.Parameters.AddWithValue("@nota", (object?)filme.NotaImdb ?? DBNull.Value);
}

static string? ValidarFilme(FilmeEntrada filme)
{
    if (filme.NotaImdb is < 0 or > 9.9m)
    {
        return "A nota IMDb deve estar entre 0 e 9,9.";
    }

    var erros = new List<ValidationResult>();
    return Validator.TryValidateObject(filme, new ValidationContext(filme), erros, true)
        ? null
        : erros[0].ErrorMessage;
}

static Filme LerFilme(MySqlDataReader reader) =>
    new(
        reader.GetInt32(0),
        reader.GetString(1),
        reader.GetString(2),
        reader.GetString(3),
        reader.GetString(4),
        reader.GetString(5),
        reader.IsDBNull(6) ? null : reader.GetInt16(6),
        reader.IsDBNull(7) ? null : reader.GetString(7),
        reader.IsDBNull(8) ? null : reader.GetString(8),
        reader.IsDBNull(9) ? null : reader.GetDecimal(9));

sealed record FilmeEntrada
{
    [Required, StringLength(200)]
    public string Titulo { get; init; } = "";

    [Required, StringLength(200)]
    public string Generos { get; init; } = "";

    [Required, StringLength(40)]
    public string Duracao { get; init; } = "";

    [Required]
    public string Sinopse { get; init; } = "";

    [Required, StringLength(255)]
    public string Imagem { get; init; } = "";

    [Range(1888, 2100)]
    public short? Ano { get; init; }

    [StringLength(200)]
    public string? Diretor { get; init; }

    [StringLength(20)]
    public string? Classificacao { get; init; }

    public decimal? NotaImdb { get; init; }
}

sealed record Filme(
    int Id,
    string Titulo,
    string Generos,
    string Duracao,
    string Sinopse,
    string Imagem,
    short? Ano,
    string? Diretor,
    string? Classificacao,
    decimal? NotaImdb);