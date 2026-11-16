const formulario = document.querySelector("#formulario-filme");
const listaFilmes = document.querySelector("#filmes-admin");
const statusLista = document.querySelector("#lista-status");
const statusFormulario = document.querySelector("#formulario-status");
const botaoSalvar = document.querySelector("#salvar-filme");
const botaoCancelar = document.querySelector("#cancelar-edicao");

async function requisitar(url, opcoes = {}) {
  const resposta = await fetch(url, {
    ...opcoes,
    headers: {
      "Content-Type": "application/json",
      ...opcoes.headers,
    },
  });

  if (resposta.status === 401) {
    window.location.replace("/login.html");
    return null;
  }

  if (!resposta.ok) {
    const erro = await resposta.json().catch(() => ({}));
    throw new Error(erro.message || erro.title || "Não foi possível concluir a operação.");
  }

  return resposta.status === 204 ? null : resposta.json();
}

function mostrarStatus(elemento, mensagem) {
  elemento.textContent = mensagem;
  elemento.hidden = !mensagem;
}

function criarBotao(texto, classe, acao) {
  const botao = document.createElement("button");
  botao.type = "button";
  botao.className = classe;
  botao.textContent = texto;
  botao.addEventListener("click", acao);
  return botao;
}

function mostrarFilmes(filmes) {
  listaFilmes.replaceChildren();

  if (filmes.length === 0) {
    statusLista.textContent = "Nenhum filme cadastrado.";
    return;
  }

  statusLista.textContent = `${filmes.length} filme(s) cadastrado(s).`;

  for (const filme of filmes) {
    const cartao = document.createElement("article");
    cartao.className = "dashboard-filme";

    const informacoes = document.createElement("div");
    const titulo = document.createElement("h2");
    titulo.textContent = filme.titulo;
    const detalhes = document.createElement("p");
    detalhes.textContent = [filme.ano, filme.generos, filme.duracao]
      .filter(Boolean)
      .join(" · ");
    informacoes.append(titulo, detalhes);

    const acoes = document.createElement("div");
    acoes.className = "dashboard-acoes";
    acoes.append(
      criarBotao("Editar", "secondary-btn", () => iniciarEdicao(filme)),
      criarBotao("Excluir", "delete-btn", () => excluirFilme(filme))
    );
    cartao.append(informacoes, acoes);
    listaFilmes.append(cartao);
  }
}

function iniciarEdicao(filme) {
  for (const campo of formulario.elements) {
    if (campo.name && Object.hasOwn(filme, campo.name)) {
      campo.value = filme[campo.name] ?? "";
    }
  }

  formulario.elements.id.value = filme.id;
  document.querySelector("#titulo-formulario").textContent = "Editar filme";
  botaoSalvar.textContent = "Salvar alterações";
  botaoCancelar.hidden = false;
  mostrarStatus(statusFormulario, "");
  formulario.scrollIntoView({ behavior: "smooth" });
}

function limparFormulario() {
  formulario.reset();
  formulario.elements.id.value = "";
  document.querySelector("#titulo-formulario").textContent = "Novo filme";
  botaoSalvar.textContent = "Adicionar filme";
  botaoCancelar.hidden = true;
  mostrarStatus(statusFormulario, "");
}

async function carregarDashboard() {
  try {
    const sessao = await requisitar("/api/admin/sessao");
    if (!sessao) return;
    document.querySelector("#admin-nome").textContent = sessao.nome;

    const filmes = await requisitar("/api/admin/filmes");
    if (filmes) mostrarFilmes(filmes);
  } catch (erro) {
    mostrarStatus(statusLista, erro.message);
  }
}

formulario.addEventListener("submit", async (evento) => {
  evento.preventDefault();
  const id = formulario.elements.id.value;
  const filme = {
    titulo: formulario.elements.titulo.value.trim(),
    generos: formulario.elements.generos.value.trim(),
    duracao: formulario.elements.duracao.value.trim(),
    sinopse: formulario.elements.sinopse.value.trim(),
    imagem: formulario.elements.imagem.value.trim(),
    ano: formulario.elements.ano.value ? Number(formulario.elements.ano.value) : null,
    diretor: formulario.elements.diretor.value.trim() || null,
    classificacao: formulario.elements.classificacao.value.trim() || null,
    notaImdb: formulario.elements.notaImdb.value
      ? Number(formulario.elements.notaImdb.value)
      : null,
  };

  try {
    await requisitar(
      id ? `/api/admin/filmes/${id}` : "/api/admin/filmes",
      {
        method: id ? "PUT" : "POST",
        body: JSON.stringify(filme),
      }
    );
    limparFormulario();
    mostrarStatus(statusFormulario, id ? "Filme atualizado." : "Filme adicionado.");
    await carregarDashboard();
  } catch (erro) {
    mostrarStatus(statusFormulario, erro.message);
  }
});

async function excluirFilme(filme) {
  if (!window.confirm(`Excluir "${filme.titulo}"?`)) return;

  try {
    await requisitar(`/api/admin/filmes/${filme.id}`, { method: "DELETE" });
    mostrarStatus(statusLista, "Filme excluído.");
    await carregarDashboard();
  } catch (erro) {
    mostrarStatus(statusLista, erro.message);
  }
}

botaoCancelar.addEventListener("click", limparFormulario);
carregarDashboard();
