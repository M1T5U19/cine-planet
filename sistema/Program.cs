using MySqlConnector;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "cineplanetSite")
});
builder.Services.AddProblemDetails();
var app = builder.Build();
var connectionString = builder.Configuration.GetConnectionString("CinePlanet")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:CinePlanet.");
const string columns = "id, titulo, generos, duracao, sinopse, imagem, ano, diretor, classificacao, nota_imdb";

app.UseExceptionHandler();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/filmes", async (CancellationToken cancellationToken) =>
{
    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);
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
    await using var command = new MySqlCommand(query, connection);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    var filmes = new List<Filme>();
    while (await reader.ReadAsync(cancellationToken))
    {
        filmes.Add(LerFilme(reader));
    }

    return filmes;
});

app.MapGet("/api/filmes/{id:int}", BuscarFilme);

app.Run();

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