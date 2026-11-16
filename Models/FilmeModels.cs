using System.ComponentModel.DataAnnotations;

namespace Models
{
    public sealed record FilmeEntrada
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

    public sealed record Filme(
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
}
