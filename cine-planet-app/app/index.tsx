import { useEffect, useState } from "react";
import Constants from "expo-constants";
import {
  FlatList,
  Image,
  Platform,
  SafeAreaView,
  StyleSheet,
  Text,
  View,
} from "react-native";

type Filme = {
  id: number;
  titulo: string;
  generos: string;
  duracao: string;
  imagem: string;
};

const API_PORT = "5292";

function obterHostDaMaquina() {
  const hostUri = Constants.expoConfig?.hostUri;
  if (!hostUri) return null;
  return hostUri.split(":")[0];
}

function obterUrlDaApi() {
  const host = obterHostDaMaquina();

  if (Platform.OS === "android") {
    return `http://${host ?? "10.0.2.2"}:${API_PORT}`;
  }

  if (Platform.OS === "ios" && host) {
    return `http://${host}:${API_PORT}`;
  }

  return `http://localhost:${API_PORT}`;
}

const API_BASE_URL = obterUrlDaApi();
const API_URL = `${API_BASE_URL}/api/filmes`;

function obterUrlDaImagem(imagem: string) {
  if (!imagem) return null;
  if (/^https?:\/\//i.test(imagem)) return imagem;
  return `${API_BASE_URL}${imagem.startsWith("/") ? "" : "/"}${imagem}`;
}

export default function App() {
  const [filmes, setFilmes] = useState<Filme[]>([]);
  const [mensagem, setMensagem] = useState("Carregando filmes...");
  const [imagensComErro, setImagensComErro] = useState<Set<number>>(new Set());

  useEffect(() => {
    fetch(API_URL)
      .then((resposta) => {
        if (!resposta.ok) throw new Error("Falha ao carregar os filmes.");
        return resposta.json();
      })
      .then((dados: Filme[]) => {
        setFilmes(dados);
        setMensagem("");
      })
      .catch(() => setMensagem("Não foi possível conectar à API."));
  }, []);

  return (
    <SafeAreaView style={styles.pagina}>
      <Text style={styles.titulo}>Cine-Planet</Text>
      {mensagem ? <Text style={styles.mensagem}>{mensagem}</Text> : null}
      <FlatList
        data={filmes}
        keyExtractor={(filme) => String(filme.id)}
        renderItem={({ item }) => (
          <View style={styles.filme}>
            {imagensComErro.has(item.id) ? (
              <View style={[styles.imagem, styles.imagemIndisponivel]}>
                <Text style={styles.textoImagem}>Sem imagem</Text>
              </View>
            ) : (
              <Image
                source={{ uri: obterUrlDaImagem(item.imagem) ?? undefined }}
                style={styles.imagem}
                resizeMode="cover"
                onError={() =>
                  setImagensComErro((atuais) => new Set(atuais).add(item.id))
                }
              />
            )}
            <View>
              <Text style={styles.texto}>{item.titulo}</Text>
              <Text style={styles.detalhe}>{item.generos}</Text>
              <Text style={styles.detalhe}>{item.duracao}</Text>
            </View>
          </View>
        )}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  pagina: { flex: 1, backgroundColor: "#0a0a0a", padding: 16 },
  titulo: { color: "#c9a84c", fontSize: 24, marginBottom: 16 },
  mensagem: { color: "#cccccc", marginBottom: 12 },
  filme: { flexDirection: "row", marginBottom: 16 },
  imagem: { width: 80, height: 120, marginRight: 12 },
  imagemIndisponivel: {
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: "#242424",
  },
  textoImagem: { color: "#777777", fontSize: 12, textAlign: "center" },
  texto: { color: "#f5f0e8", fontSize: 18 },
  detalhe: { color: "#aaaaaa" },
});
