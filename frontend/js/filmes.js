const grid = document.querySelector("#filmes-grid");
const statusMessage = document.querySelector("#filmes-status");

if (grid && statusMessage) {
  const destaques = [
    1,
    2,
    3,
    4,
  ];

  function criarCard(filme) {
    const card = document.createElement("a");
    card.className = "card";
    card.href = `/filme.html?id=${encodeURIComponent(filme.id)}`;
    card.setAttribute("aria-label", `Ver ${filme.titulo}`);

    const imagem = document.createElement("img");
    imagem.src = filme.imagem;
    imagem.alt = `Cartaz de ${filme.titulo}`;

    const titulo = document.createElement("h3");
    titulo.textContent = filme.titulo;

    const resumo = document.createElement("p");
    resumo.textContent = `${filme.generos} · ${filme.duracao}`;

    card.append(imagem, titulo, resumo);
    return card;
  }

  fetch("/api/filmes")
    .then((response) => {
      if (!response.ok) throw new Error("Falha ao carregar os filmes.");
      return response.json();
    })
    .then((filmes) => {
      const lista = grid.dataset.filmes === "destaques"
        ? destaques.map((id) => filmes.find((filme) => filme.id === id)).filter(Boolean)
        : filmes;
      grid.replaceChildren(...lista.map(criarCard));
      statusMessage.hidden = true;
    })
    .catch(() => {
      statusMessage.textContent = "Não foi possível carregar os filmes. Verifique a conexão com a API e o banco de dados.";
    });
}
