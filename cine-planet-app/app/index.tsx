import { useEffect, useState } from "react";
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

const API_URL = Platform.OS === "android"
  ? "http://10.0.2.2:5292/api/filmes"
  : "http://localhost:5292/api/filmes";

export default function App() {
  const [filmes, setFilmes] = useState<Filme[]>([]);
  const [mensagem, setMensagem] = useState("Carregando filmes...");

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
            <Image source={{ uri: item.imagem }} style={styles.imagem} />
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
  texto: { color: "#f5f0e8", fontSize: 18 },
  detalhe: { color: "#aaaaaa" },
});
