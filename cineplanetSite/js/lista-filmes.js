const grid = document.querySelector("#filmes-grid");
const statusMessage = document.querySelector("#filmes-status");
const filmesEmDestaque = [1, 2, 3, 4];

const criarTexto = (tag, text) => {
  const elemento = document.createElement(tag);
  elemento.textContent = text;
  return elemento;
};

function criarCard(filme) {
  const card = document.createElement("a");
  card.className = "card";
  card.href = `/filme.html?id=${encodeURIComponent(filme.id)}`;
  card.setAttribute("aria-label", `Ver ${filme.titulo}`);

  const imagem = document.createElement("img");
  imagem.src = filme.imagem;
  imagem.alt = `Cartaz de ${filme.titulo}`;
  card.append(
    imagem,
    criarTexto("h3", filme.titulo),
    criarTexto("p", `${filme.generos} · ${filme.duracao}`)
  );
  return card;
}

async function carregarFilmes() {
  try {
    const response = await fetch("/api/filmes");
    if (!response.ok) {
      throw new Error("Falha ao carregar os filmes.");
    }

    const filmes = await response.json();
    const lista = grid.dataset.filmes === "destaques"
      ? filmesEmDestaque
          .map((id) => filmes.find((filme) => filme.id === id))
          .filter(Boolean)
      : filmes;
    grid.replaceChildren(...lista.map(criarCard));
    statusMessage.hidden = true;
  } catch {
    statusMessage.textContent = "Não foi possível carregar os filmes. Verifique a conexão com a API e o banco de dados.";
  }
}

if (grid && statusMessage) {
  carregarFilmes();
}
