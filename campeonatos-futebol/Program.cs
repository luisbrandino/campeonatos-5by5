using campeonatos_futebol;
using campeonatos_futebol.Models;

Menu menu = new Menu(
    "Criar novo time",
    "Criar novo campeonato",
    "Criar novo jogo",
    "Sair"
);

menu.DefinirTitulo("CAMPEONATOS");
menu.LimparAposImpressao(true);

void criarTime()
{
    Time time = new();

    if (time.Existe(new Dictionary<string, object> { { "nome", "Titanos"} }))
    {
        Console.WriteLine("Time já existe");
        Console.ReadKey();
        return;
    }

    time.Inserir(new Dictionary<string, object>
    {
        { "nome", "Titanos" },
        { "apelido", "Titans" },
        { "data_criacao", DateTime.Now },
    });
}

void criarCampeonato()
{
    Campeonato campeonato = new();
    string nome = "Liga Americana";

    if (campeonato.Existe(nome))
    {
        Console.WriteLine("Campeonato já existe");
        Console.ReadKey();
        return;
    }

    campeonato.Inserir(new Dictionary<string, object>()
    {
        { "nome", nome }
    });
}

while (true)
{
    switch (menu.Perguntar())
    {
        case 1:
            criarTime();
            break;
        case 2:
            criarCampeonato();
            break;
        case 3:
            break;
        default:
            Environment.Exit(0);
            break;
    }
}