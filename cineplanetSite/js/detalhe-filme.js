const filmeId = new URLSearchParams(window.location.search).get("id");
const statusMessage = document.querySelector("#filme-status");
const content = document.querySelector("#filme-detalhe");
const errorMessage = document.querySelector("#filme-erro");

function mostrarErro(titulo) {
  statusMessage.hidden = true;
  document.querySelector("#erro-titulo").textContent = titulo;
  errorMessage.hidden = false;
}

if (!filmeId || !/^[1-9]\d*$/.test(filmeId)) {
  mostrarErro("ID inválido");
} else {
  fetch(`/api/filmes/${encodeURIComponent(filmeId)}`)
    .then(async (response) => {
      if (response.status === 404) {
        throw new Error("Filme não encontrado");
      }

      if (!response.ok) {
        throw new Error("Não foi possível carregar o filme");
      }
      return response.json();
    })
    .then((filme) => {
      document.title = `${filme.titulo} | Cine-Planet`;
      document.querySelector("#filme-imagem").src = filme.imagem;
      document.querySelector("#filme-imagem").alt = `Cartaz de ${filme.titulo}`;
      document.querySelector("#filme-titulo").textContent = filme.titulo;
      document.querySelector("#filme-generos").textContent = filme.generos;
      document.querySelector("#filme-duracao").textContent = filme.duracao;
      document.querySelector("#filme-ano").textContent = filme.ano ?? "Não informado";
      document.querySelector("#filme-diretor").textContent = filme.diretor ?? "Não informado";
      document.querySelector("#filme-classificacao").textContent =
        filme.classificacao ?? "Não informada";
      document.querySelector("#filme-nota").textContent =
        filme.notaImdb == null ? "Não informada" : `${filme.notaImdb.toFixed(1)}/10`;
      document.querySelector("#filme-sinopse").textContent = filme.sinopse;
      content.hidden = false;
      statusMessage.hidden = true;
    })
    .catch((error) => mostrarErro(error.message));
}
