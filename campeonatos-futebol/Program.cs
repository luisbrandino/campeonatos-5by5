using campeonatos_futebol;
using campeonatos_futebol.Models;
using System;
using System.Data;

Menu menu = new Menu(
    "Criar novo time",
    "Criar novo campeonato",
    "Buscar campeão",
    "Buscar time com mais gols",
    "Buscar time que tomou mais gols",
    "Buscar jogo com mais gols",
    "Buscar maior número de gols de cada time",
    "Buscar times do campeonato",
    "Sair"
);

menu.DefinirTitulo("CAMPEONATOS");
menu.LimparAposImpressao(true);

int criarMenuDeSelecao(DataRowCollection entidades, string? titulo = null)
{
    Menu selecao = new();

    titulo ??= "SELEÇÃO";

    selecao.DefinirTitulo(titulo);
    selecao.LimparAposImpressao(true);

    foreach (DataRow entidade in entidades)
        selecao.AdicionarOpcao((string) entidade["nome"]);

    return selecao.Perguntar() - 1;
}

void imprimirTime(DataRow time)
{
    Console.WriteLine($"ID: {time["id"]}\nNome: {time["nome"]}\nApelido: {time["apelido"]}\nData de criação: {((DateTime)time["data_criacao"]).ToString("dd/MM/yyyy")}");
}

void criarTime()
{
    Console.Clear();
    Time time = new();

    Entrada<string> entradaNome = new();

    entradaNome.AdicionarRegra((string nome) => nome.Length <= 30, "Nome não pode ter mais que 30 caracteres");
    entradaNome.AdicionarRegra((string nome) => !time.Existe(nome), "Time já existe");

    Console.Write("Informe o nome do time: ");
    string nome = entradaNome.Pegar();

    Entrada<string> entradaApelido = new();

    entradaApelido.AdicionarRegra((string nome) => nome.Length <= 30, "Apelido não pode ter mais que 30 caracteres");

    Console.Write("Informe o apelido do time: ");
    string apelido = entradaApelido.Pegar();

    int id = time.Inserir(new Dictionary<string, object>
    {
        { "nome", nome },
        { "apelido", apelido },
        { "data_criacao", DateTime.Now.AddYears(new Random().Next(-40, -10))},
    });

    Console.WriteLine($"Time {nome} criado");
    Console.ReadKey();
}

void criarCampeonato()
{
    Console.Clear();
    Time time = new();
    int quantidadeDeTimes = time.Contagem();

    if (quantidadeDeTimes < 3)
    {
        Console.WriteLine("Não há times suficientes para criar um campeonato. (No mínimo 3)");
        Console.ReadKey();
        return;
    }

    Campeonato campeonato = new();

    Entrada<string> entrada = new();

    entrada.AdicionarRegra((string nome) => nome.Length <= 30, "Nome não pode ter mais que 30 caracteres");
    entrada.AdicionarRegra((string nome) => !campeonato.Existe(nome), "Campeonato já existe");

    Console.Write("Informe o nome do campeonato: ");
    string nome = entrada.Pegar();

    int campeonatoId = campeonato.Inserir(new Dictionary<string, object>()
    {
        { "nome", nome }
    });

    Entrada<int> entradaQuantidadeDeParticipantes = new();

    entradaQuantidadeDeParticipantes.AdicionarRegra((int quantidade) => quantidade >= 3 && quantidade <= 5, "Quantidade de participantes tem que ser entre 3 e 5");
    entradaQuantidadeDeParticipantes.AdicionarRegra((int quantidade) => quantidade <= quantidadeDeTimes, $"Só há {quantidadeDeTimes} times cadastrados");

    Console.Write("Informe a quantidade de participantes do campeonato (mínimo 3 e máximo 5)");
    int quantidadeDeParticipantes = entradaQuantidadeDeParticipantes.Pegar();

    DataRowCollection times = time.BuscarTodos();
    List<int> timesEscolhidos = new();

    for (int i = 0; i < quantidadeDeParticipantes; i++)
    {
        int timeEscolhido = criarMenuDeSelecao(times, $"Selecione o {i + 1}º time");

        int timeId = (int) times[timeEscolhido]["id"];

        Participante participante = new();

        participante.Inserir(new Dictionary<string, object>
        {
            { "campeonato_id", campeonatoId },
            { "time_id", timeId }
        });

        times.RemoveAt(timeEscolhido);
        timesEscolhidos.Add(timeId);
    }

    Console.Clear();

    int quantidadeJogos = quantidadeDeParticipantes * quantidadeDeParticipantes - quantidadeDeParticipantes;

    foreach (int timeMandanteId in timesEscolhidos)
        foreach(int timeVisitanteId in timesEscolhidos)
        {
            if (timeMandanteId == timeVisitanteId)
                continue;

            Jogo jogo = new();

            jogo.Inserir(new Dictionary<string, object>
            {
                { "campeonato_id", campeonatoId },
                { "time_mandante_id", timeMandanteId },
                { "time_visitante_id", timeVisitanteId }
            });

            jogo.DefinirGolsTimeMandante(new Dictionary<string, object>
            {
                { "campeonato_id", campeonatoId },
                { "time_mandante_id", timeMandanteId },
                { "time_visitante_id", timeVisitanteId },
                { "gols", new Random().Next(0, 10) }
            });

            jogo.DefinirGolsTimeVisitante(new Dictionary<string, object>
            {
                { "campeonato_id", campeonatoId },
                { "time_mandante_id", timeMandanteId },
                { "time_visitante_id", timeVisitanteId },
                { "gols", new Random().Next(0, 10) }
            });
        }

    Console.WriteLine("Campeonato criado!");
    Console.ReadKey();
}

void buscarCampeao()
{
    Console.Clear();
    Campeonato campeonato = new();

    if (campeonato.Contagem() <= 0)
    {
        Console.WriteLine("Não há campeonatos cadastrados");
        Console.ReadKey();
        return;
    }

    DataRowCollection campeonatos = campeonato.BuscarTodos();

    DataRow campeonatoEscolhido = campeonatos[criarMenuDeSelecao(campeonatos, "Selecione o campeonato")];

    Console.Clear();

    Time time = new();

    DataRow timeCampeao  = time.Buscar((int)campeonatoEscolhido["time_campeao_id"])[0];

    Console.WriteLine($"Time campeão:\n");
    imprimirTime(timeCampeao);
    Console.ReadKey();
}

void buscarTimeQueMaisFezGol()
{
    Console.Clear();
    Campeonato campeonato = new();

    if (campeonato.Contagem() <= 0)
    {
        Console.WriteLine("Não há campeonatos cadastrados");
        Console.ReadKey();
        return;
    }

    DataRowCollection campeonatos = campeonato.BuscarTodos();

    DataRow campeonatoEscolhido = campeonatos[criarMenuDeSelecao(campeonatos, "Selecione o campeonato")];

    Console.Clear();

    DataRow timeComMaisGols = campeonato.BuscarTimeComMaisGols((int)campeonatoEscolhido["id"]);

    Console.WriteLine($"Time com mais gols:\n");
    imprimirTime(timeComMaisGols);
    Console.WriteLine($"Gols feitos: {timeComMaisGols["total_gols"]}");
    Console.ReadKey();
}

void buscarTimeQueMaisTomouGols()
{
    Console.Clear();
    Campeonato campeonato = new();

    if (campeonato.Contagem() <= 0)
    {
        Console.WriteLine("Não há campeonatos cadastrados");
        Console.ReadKey();
        return;
    }

    DataRowCollection campeonatos = campeonato.BuscarTodos();

    DataRow campeonatoEscolhido = campeonatos[criarMenuDeSelecao(campeonatos, "Selecione o campeonato")];

    Console.Clear();

    DataRow gols = campeonato.BuscarTimeQueTomouMaisGols((int)campeonatoEscolhido["id"]);
    int timeId = (int)gols["time_id"];
    
    Time time = new();

    DataRow timeQueTomouMaisGols = time.Buscar(timeId)[0];

    Console.WriteLine($"Time que tomou mais gols:\n");
    imprimirTime(timeQueTomouMaisGols);
    Console.WriteLine($"Gols tomados: {gols["total_gols_sofridos"]}");
    Console.ReadKey();
    Console.Clear();
}

void buscarJogoComMaisGols()
{
    Console.Clear();
    Campeonato campeonato = new();

    if (campeonato.Contagem() <= 0)
    {
        Console.WriteLine("Não há campeonatos cadastrados");
        Console.ReadKey();
        return;
    }

    DataRowCollection campeonatos = campeonato.BuscarTodos();

    DataRow campeonatoEscolhido = campeonatos[criarMenuDeSelecao(campeonatos, "Selecione o campeonato")];

    Console.Clear();

    DataRow jogo = campeonato.BuscarJogoComMaisGols((int)campeonatoEscolhido["id"]);

    Time time = new();

    DataRow timeMandante = time.Buscar((int)jogo["time_mandante_id"])[0];
    DataRow timeVisitante = time.Buscar((int)jogo["time_visitante_id"])[0];

    Console.WriteLine("Jogo com mais gols: \n");

    Console.WriteLine("Time mandante: ");
    imprimirTime(timeMandante);
    Console.WriteLine($"Quantidade de gols: {jogo["gols_time_mandante"]}");

    Console.WriteLine("\nTime visitante: ");
    imprimirTime(timeVisitante);
    Console.WriteLine($"Quantidade de gols: {jogo["gols_time_visitante"]}");

    Console.ReadKey();
    Console.Clear();
}

void buscarMaiorNumeroDeGolsDeCadaTime()
{
    Console.Clear();
    Campeonato campeonato = new();

    if (campeonato.Contagem() <= 0)
    {
        Console.WriteLine("Não há campeonatos cadastrados");
        Console.ReadKey();
        return;
    }

    DataRowCollection campeonatos = campeonato.BuscarTodos();

    DataRow campeonatoEscolhido = campeonatos[criarMenuDeSelecao(campeonatos, "Selecione o campeonato")];

    Console.Clear();

    DataRowCollection times = campeonato.BuscarMaiorNumeroDeGolsDeCadaTime((int)campeonatoEscolhido["id"]);

    for (int i = 0; i < times.Count; i++)
    {
        Console.Write($"Time {i+1}:\n");
        imprimirTime(times[i]);
        Console.Write($"Gols: {times[i]["max_gols"]}\n\n");
    }

    Console.ReadKey();
    Console.Clear();
}

void buscarTimesDoCampeonato()
{
    Console.Clear();
    Campeonato campeonato = new();

    if (campeonato.Contagem() <= 0)
    {
        Console.WriteLine("Não há campeonatos cadastrados");
        Console.ReadKey();
        return;
    }

    DataRowCollection campeonatos = campeonato.BuscarTodos();

    DataRow campeonatoEscolhido = campeonatos[criarMenuDeSelecao(campeonatos, "Selecione o campeonato")];

    Console.Clear();

    Participante participante = new();

    DataRowCollection participantes = participante.Buscar((int) campeonatoEscolhido["id"]);

    Time time = new();

    int index = 1;
    foreach (DataRow participanteDoCampeonato in participantes)
    {
        DataRow timeParticipante = time.Buscar((int)participanteDoCampeonato["time_id"])[0];

        Console.Write($"{index++}º participante:\n");
        imprimirTime(timeParticipante);
        Console.WriteLine($"Pontos: {participanteDoCampeonato["pontos"]}");
        Console.WriteLine($"Total de gols: {participanteDoCampeonato["total_gols"]}\n\n");
    }

    Console.ReadKey();
    Console.Clear();
}

while (true)
{
    // por alguma razão, o buffer não é limpo corretamente se o conteudo dele for maior que o tamanho da janela, atrapalhando as próximas impressões
    // vi essa resolução no stackoverflow e funcionou
    // aparantemente, esse comando faz o mesmo que o Console.Clear(), porém enviando diretamente a sequencia de escape ansi para limpar o console e retirar a scrollbar
    Console.WriteLine("\x1b[3J");
    switch (menu.Perguntar())
    {
        case 1:
            criarTime();
            break;
        case 2:
            criarCampeonato();
            break;
        case 3:
            buscarCampeao();
            break;
        case 4:
            buscarTimeQueMaisFezGol();
            break;
        case 5:
            buscarTimeQueMaisTomouGols();
            break;
        case 6:
            buscarJogoComMaisGols();
            break;
        case 7:
            buscarMaiorNumeroDeGolsDeCadaTime();
            break;
        case 8:
            buscarTimesDoCampeonato();
            break;
        default:
            Environment.Exit(0);
            break;
    }
}