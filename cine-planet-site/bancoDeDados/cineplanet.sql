CREATE DATABASE IF NOT EXISTS cineplanet;
USE cineplanet;

CREATE TABLE IF NOT EXISTS filmes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  titulo VARCHAR(200) NOT NULL UNIQUE,
  generos VARCHAR(200) NOT NULL,
  duracao VARCHAR(40) NOT NULL,
  sinopse TEXT NOT NULL,
  imagem VARCHAR(255) NOT NULL,
  ano SMALLINT,
  diretor VARCHAR(200),
  classificacao VARCHAR(20),
  nota_imdb DECIMAL(2,1)
);

INSERT INTO filmes (
  titulo,
  generos,
  duracao,
  sinopse,
  imagem,
  ano,
  diretor,
  classificacao,
  nota_imdb
) VALUES
  (
    'Vingadores: Ultimato', 'Ação · Aventura · Ficção científica', '3h 1min',
    'Após uma derrota devastadora, os heróis que restaram precisam encontrar uma forma de reparar as perdas e trazer esperança de volta ao universo. A missão reúne antigos aliados e coloca à prova a coragem, a amizade e os sacrifícios de cada um.',
    '/assets/images/Avengers_Endgame.jpg', 2019, 'Anthony Russo, Joe Russo', 'PG-13', 8.4
  ),
  (
    'Jurassic Park', 'Aventura · Ficção científica', '2h 7min',
    'Um parque temático criado para exibir dinossauros recriados a partir de material genético recebe visitantes antes de estar totalmente pronto. Quando os sistemas de segurança falham, especialistas e crianças precisam atravessar o local e encontrar uma saída enquanto os animais circulam livremente.',
    '/assets/images/Jurassic_Park_.jpg', 1993, 'Steven Spielberg', 'PG-13', 8.2
  ),
  (
    'Avatar: O Caminho da Água', 'Ficção científica · Aventura', '3h 12min',
    'Jake e Neytiri construíram uma vida em Pandora e agora precisam proteger seus filhos quando uma antiga ameaça retorna. Para encontrar abrigo, a família se aproxima de um povo ligado ao oceano, aprende seus costumes e enfrenta perigos que colocam em risco tanto seu lar quanto a comunidade que os acolheu.',
    '/assets/images/avatar2.jpeg', 2022, 'James Cameron', 'PG-13', 7.5
  ),
  (
    'Titanic', 'Romance · Drama', '3h 14min',
    'Durante a viagem inaugural do Titanic, Rose, uma jovem presa às expectativas da alta sociedade, conhece Jack, um artista de origem humilde. A relação entre os dois desafia as diferenças sociais e se desenvolve enquanto o navio segue em direção a um desastre que mudará suas vidas.',
    '/assets/images/Titanic.jpg', 1997, 'James Cameron', 'PG-13', 8.0
  ),
  (
    'Clube da Luta', 'Drama · Suspense', '2h 19min',
    'Cansado de uma rotina sem sentido e afetado pela insônia, um funcionário conhece Tyler Durden, um homem que rejeita as convenções sociais. Juntos, eles criam encontros clandestinos de luta que atraem cada vez mais participantes e acabam tomando um rumo imprevisível.',
    '/assets/images/Clube da luta.jpg', 1999, 'David Fincher', 'R', 8.8
  ),
  (
    'Até o Último Homem', 'Drama · Guerra', '2h 19min',
    'Inspirado na história real de Desmond Doss, o filme acompanha um médico do Exército que se recusa a portar armas por convicção pessoal. Durante a Batalha de Okinawa, ele permanece no campo de combate para socorrer soldados feridos, mesmo diante de um risco extremo.',
    '/assets/images/ate o ultimo homem.jpg', 2016, 'Mel Gibson', 'R', 8.1
  ),
  (
    'Deadpool', 'Ação · Comédia', '1h 48min',
    'Depois de ser submetido a um experimento que altera seu corpo, Wade Wilson adota uma identidade irreverente e ganha uma capacidade extraordinária de se recuperar. Enquanto tenta acertar contas com quem destruiu sua vida, ele atravessa situações violentas com humor ácido e pouca preocupação com regras.',
    '/assets/images/deadpool.jpg', 2016, 'Tim Miller', 'R', 8.0
  ),
  (
    'Super Mario Bros. O Filme', 'Animação · Aventura', '1h 32min',
    'Os encanadores Mario e Luigi são separados depois de chegar a um reino fantástico, onde Mario conhece novos aliados e aprende a lidar com desafios bem diferentes dos que conhece. Com a ajuda da Princesa Peach e de Toad, ele parte para impedir os planos de Bowser e reencontrar o irmão.',
    '/assets/images/mario.jpg', 2023, 'Aaron Horvath, Michael Jelenic, Pierre Leduc', 'PG', 7.0
  ),
  (
    'Godzilla vs. Kong', 'Ação · Ficção científica', '1h 53min',
    'Enquanto humanos investigam uma região misteriosa ligada à origem dos Titãs, Godzilla e Kong entram em rota de colisão. A disputa entre os dois gigantes revela uma ameaça maior e obriga diferentes personagens a reconsiderar o que sabem sobre as criaturas e sobre o passado da Terra.',
    '/assets/images/Godzilla_vs._Kong.jpg', 2021, 'Adam Wingard', 'PG-13', 6.3
  ),
  (
    'Carros 3', 'Animação · Esporte', '1h 42min',
    'Relâmpago McQueen enfrenta uma geração de carros mais velozes e equipados com novas tecnologias, e uma derrota o faz questionar o futuro nas pistas. Com o apoio da treinadora Cruz Ramirez, ele trabalha para recuperar a confiança e descobrir como continuar competitivo sem abandonar sua própria trajetória.',
    '/assets/images/carros 3.jpg', 2017, 'Brian Fee', 'G', 6.7
  ),
  (
    'Thor: Ragnarok', 'Ação · Aventura', '2h 10min',
    'Longe de Asgard e preso no planeta Sakaar, Thor precisa encontrar um caminho de volta antes que Hela consolide seu poder. Para impedir a destruição de seu reino, ele reúne aliados improváveis e encara desafios que o obrigam a repensar o que significa ser um herói.',
    '/assets/images/Thor_Ragnarok_poster.jpg', 2017, 'Taika Waititi', 'PG-13', 7.9
  ),
  (
    'Rota de Fuga', 'Ação · Suspense', '1h 55min',
    'Ray Breslin, especialista em testar a segurança de prisões, é enganado e enviado para uma instalação praticamente impossível de escapar. Sem poder confiar nos próprios conhecimentos sobre o lugar, ele precisa colaborar com outro detento para descobrir como sair e quem armou sua captura.',
    '/assets/images/rota de fuga.jpg', 2013, 'Mikael Håfström', 'R', 6.7
  ),
  (
    'Shrek', 'Animação · Fantasia · Comédia', '1h 30min',
    'Quando criaturas de contos de fadas são expulsas de suas casas, o ogro Shrek faz um acordo para recuperar a tranquilidade de seu pântano. Acompanhado pelo falante Burro, ele parte para resgatar a Princesa Fiona e acaba descobrindo que a aventura mudará sua visão sobre amizade e aparência.',
    '/assets/images/sherek.jpg', 2001, 'Andrew Adamson, Vicky Jenson', 'PG', 7.9
  ),
  (
    'Homem-Aranha: Sem Volta Para Casa', 'Ação · Aventura', '2h 28min',
    'Depois que sua identidade se torna pública, Peter Parker procura uma solução para recuperar a privacidade e proteger as pessoas próximas. Um feitiço que sai do controle abre passagem para visitantes de outros universos e transforma sua tentativa de voltar à vida normal em uma escolha de grandes consequências.',
    '/assets/images/homem aranha.webp', 2021, 'Jon Watts', 'PG-13', 8.1
  ),
  (
    'Filme sem identificação', 'Gênero não identificado', 'Duração não informada',
    'Ainda não foi possível identificar o título e os dados deste filme pelo cartaz disponível. As informações serão preenchidas quando o filme for identificado.',
    '/assets/images/boom.webp', NULL, NULL, NULL, NULL
  ),
  (
    'Um Filme Minecraft', 'Aventura · Fantasia', '1h 41min',
    'Quatro pessoas são levadas por um portal a um mundo cúbico onde a imaginação interfere diretamente no que é possível construir. Com a ajuda de um especialista local, elas precisam aprender a sobreviver, enfrentar perigos e encontrar uma maneira de voltar para casa.',
    '/assets/images/minecraft.webp', 2025, 'Jared Hess', 'PG', 5.6
  ),
  (
    'Toy Story 3', 'Animação · Aventura', '1h 43min',
    'Com Andy prestes a sair de casa para a faculdade, Woody e os outros brinquedos tentam entender qual será seu próximo destino. Um engano os leva a uma creche cheia de brinquedos, onde precisam se adaptar a uma nova rotina e encontrar uma forma de permanecer juntos.',
    '/assets/images/Toy Story 3.jpg', 2010, 'Lee Unkrich', 'G', 8.3
  ),
  (
    'A Era do Gelo', 'Animação · Comédia', '1h 21min',
    'No início de uma era glacial, o mamute Manny, a preguiça Sid e o tigre Diego acabam responsáveis por conduzir um bebê humano de volta à sua família. A jornada atravessa paisagens perigosas e transforma a cooperação entre companheiros muito diferentes em uma amizade improvável.',
    '/assets/images/ice age.jpg', 2002, 'Carlos Saldanha, Chris Wedge', 'PG', 7.5
  ),
  (
    'O Senhor dos Anéis: A Sociedade do Anel', 'Aventura · Fantasia', '2h 58min',
    'Frodo recebe a missão de levar um anel de poder para longe do Condado e impedir que ele volte às mãos de Sauron. Com a ajuda de companheiros de diferentes povos, ele inicia uma longa jornada pela Terra-média, enquanto forças sombrias se organizam para recuperar o objeto.',
    '/assets/images/Senhor dos anais.jpg', 2001, 'Peter Jackson', 'PG-13', 8.9
  ),
  (
    'O Máscara', 'Comédia · Fantasia', '1h 41min',
    'Stanley Ipkiss, um bancário tímido, encontra uma máscara misteriosa que transforma sua personalidade e lhe dá habilidades extraordinárias. Ao descobrir que esse poder também chama a atenção de criminosos, ele precisa equilibrar sua nova confiança com as consequências de suas escolhas.',
    '/assets/images/mascara.jpg', 1994, 'Chuck Russell', 'PG-13', 7.0
  )
ON DUPLICATE KEY UPDATE
  generos = VALUES(generos),
  duracao = VALUES(duracao),
  sinopse = VALUES(sinopse),
  imagem = VALUES(imagem),
  ano = VALUES(ano),
  diretor = VALUES(diretor),
  classificacao = VALUES(classificacao),
  nota_imdb = VALUES(nota_imdb);
