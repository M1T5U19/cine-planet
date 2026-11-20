import { useCallback, useEffect, useState } from "react";
import {
  ActivityIndicator,
  FlatList,
  Image,
  Platform,
  RefreshControl,
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

function obterUrlDaApi() {
  if (Platform.OS === "android")
    return "http://10.0.2.2:5292/api/filmes";
  return "http://localhost:5292/api/filmes";
}

export default function FilmesScreen() {
  const [filmes, setFilmes] = useState<Filme[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [atualizando, setAtualizando] = useState(false);
  const [erro, setErro] = useState("");

  const carregarFilmes = useCallback(async (mostrarAtualizacao = false) => {
    if (mostrarAtualizacao) setAtualizando(true);
    setErro("");
    try {
      const resposta = await fetch(obterUrlDaApi());
      if (!resposta.ok) throw new Error("Falha ao carregar os filmes.");
      setFilmes(await resposta.json());
    } catch {
      setErro("Não foi possível conectar ao Cine-Planet.");
    } finally {
      setCarregando(false);
      setAtualizando(false);
    }
  }, []);

  useEffect(() => {
    carregarFilmes();
  }, [carregarFilmes]);

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.logo}>CINE-PLANET</Text>
        <Text style={styles.titulo}>Em cartaz</Text>
      </View>
      {carregando ? (
        <ActivityIndicator size="large" color="#c9a84c" style={styles.status} />
      ) : erro ? (
        <Text style={styles.statusText}>{erro}</Text>
      ) : (
        <FlatList
          data={filmes}
          keyExtractor={(filme) => String(filme.id)}
          numColumns={2}
          columnWrapperStyle={styles.row}
          contentContainerStyle={styles.lista}
          refreshControl={
            <RefreshControl
              refreshing={atualizando}
              onRefresh={() => carregarFilmes(true)}
              tintColor="#c9a84c"
            />
          }
          renderItem={({ item }) => (
            <View style={styles.card}>
              {item.imagem ? (
                <Image source={{ uri: item.imagem }} style={styles.imagem} />
              ) : (
                <View style={[styles.imagem, styles.semImagem]} />
              )}
              <Text style={styles.nome}>{item.titulo}</Text>
              <Text style={styles.meta}>{item.generos}</Text>
              <Text style={styles.meta}>{item.duracao}</Text>
            </View>
          )}
          ListEmptyComponent={
            <Text style={styles.statusText}>Nenhum filme cadastrado.</Text>
          }
        />
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#0a0a0a" },
  header: { paddingHorizontal: 20, paddingTop: 16, paddingBottom: 10 },
  logo: { color: "#c9a84c", fontSize: 18, fontWeight: "800", letterSpacing: 2 },
  titulo: { color: "#f5f0e8", fontSize: 30, fontWeight: "700", marginTop: 16 },
  status: { marginTop: 40 },
  statusText: { color: "#aaa", textAlign: "center", marginHorizontal: 20, marginTop: 40 },
  lista: { paddingHorizontal: 12, paddingBottom: 30 },
  row: { justifyContent: "space-between" },
  card: { width: "48%", marginBottom: 18 },
  imagem: { width: "100%", height: 220, borderRadius: 8, backgroundColor: "#181818" },
  semImagem: { borderWidth: 1, borderColor: "#333" },
  nome: { color: "#f5f0e8", fontSize: 16, fontWeight: "700", marginTop: 8 },
  meta: { color: "#999", fontSize: 13, marginTop: 3 },
});
