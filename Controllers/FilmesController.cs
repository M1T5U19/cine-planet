using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Models;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmesController : ControllerBase
    {
        private readonly string _connectionString;
        private const string columns = "id, titulo, generos, duracao, sinopse, imagem, ano, diretor, classificacao, nota_imdb";

        public FilmesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CinePlanet") ?? throw new InvalidOperationException("Configure ConnectionStrings:CinePlanet.");
        }

        [HttpGet]
        public async Task<ActionResult<List<Filme>>> GetFilmes(CancellationToken cancellationToken)
        {
            var query = $@"SELECT {columns} FROM filmes AS f WHERE f.id = (SELECT MIN(id) FROM filmes WHERE titulo = f.titulo) ORDER BY f.titulo";
            return await BuscarFilmes(query, cancellationToken);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Filme>> GetFilme(int id, CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new MySqlCommand($"SELECT {columns} FROM filmes WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
                return LerFilme(reader);
            return NotFound(new { message = "Não foi possível localizar o filme." });
        }

        [HttpPost]
        public async Task<IActionResult> CriarFilme([FromBody] FilmeEntrada filme, CancellationToken cancellationToken)
        {
            var erro = ValidarFilme(filme);
            if (erro is not null)
                return BadRequest(new { message = erro });
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new MySqlCommand(@"INSERT INTO filmes (titulo, generos, duracao, sinopse, imagem, ano, diretor, classificacao, nota_imdb) VALUES (@titulo, @generos, @duracao, @sinopse, @imagem, @ano, @diretor, @classificacao, @nota)", connection);
            AdicionarParametros(command, filme);
            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (MySqlException exception) when (exception.Number == 1062)
            {
                return Conflict(new { message = "Já existe um filme com esse título." });
            }
            var id = checked((int)command.LastInsertedId);
            return Created($"/api/filmes/{id}", new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarFilme(int id, [FromBody] FilmeEntrada filme, CancellationToken cancellationToken)
        {
            var erro = ValidarFilme(filme);
            if (erro is not null)
                return BadRequest(new { message = erro });
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new MySqlCommand(@"UPDATE filmes SET titulo = @titulo, generos = @generos, duracao = @duracao, sinopse = @sinopse, imagem = @imagem, ano = @ano, diretor = @diretor, classificacao = @classificacao, nota_imdb = @nota WHERE id = @id", connection);
            AdicionarParametros(command, filme);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                if (await command.ExecuteNonQueryAsync(cancellationToken) == 0)
                    return NotFound(new { message = "Não foi possível localizar o filme." });
            }
            catch (MySqlException exception) when (exception.Number == 1062)
            {
                return Conflict(new { message = "Já existe um filme com esse título." });
            }
            return Ok(new { message = "Filme atualizado." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> ExcluirFilme(int id, CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new MySqlCommand("DELETE FROM filmes WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            if (await command.ExecuteNonQueryAsync(cancellationToken) == 0)
                return NotFound(new { message = "Não foi possível localizar o filme." });
            return NoContent();
        }

        private static void AdicionarParametros(MySqlCommand command, FilmeEntrada filme)
        {
            command.Parameters.AddWithValue("@titulo", filme.Titulo.Trim());
            command.Parameters.AddWithValue("@generos", filme.Generos.Trim());
            command.Parameters.AddWithValue("@duracao", filme.Duracao.Trim());
            command.Parameters.AddWithValue("@sinopse", filme.Sinopse.Trim());
            command.Parameters.AddWithValue("@imagem", filme.Imagem.Trim());
            command.Parameters.AddWithValue("@ano", (object?)filme.Ano ?? DBNull.Value);
            command.Parameters.AddWithValue("@diretor", string.IsNullOrWhiteSpace(filme.Diretor) ? DBNull.Value : filme.Diretor.Trim());
            command.Parameters.AddWithValue("@classificacao", string.IsNullOrWhiteSpace(filme.Classificacao) ? DBNull.Value : filme.Classificacao.Trim());
            command.Parameters.AddWithValue("@nota", (object?)filme.NotaImdb ?? DBNull.Value);
        }

        private static string? ValidarFilme(FilmeEntrada filme)
        {
            if (filme.NotaImdb is < 0 or > 9.9m)
                return "A nota IMDb deve estar entre 0 e 9,9.";
            var erros = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(filme, new System.ComponentModel.DataAnnotations.ValidationContext(filme), erros, true)
                ? null
                : erros[0].ErrorMessage;
        }

        private static Filme LerFilme(MySqlDataReader reader) =>
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
    }
}
