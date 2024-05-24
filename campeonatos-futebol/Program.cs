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
    if (Time.Existe("Titanos"))
    {
        Console.WriteLine("Time já existe");
        Console.ReadKey();
        return;
    }

    Time.Inserir(new Dictionary<string, object>
    {
        { "nome", "Titanos" },
        { "apelido", "Titans" },
        { "data_criacao", DateTime.Now },
    });
}

void criarCampeonato()
{
    string nome = "Liga Americana";

    if (Campeonato.Existe(nome))
    {
        Console.WriteLine("Campeonato já existe");
        Console.ReadKey();
        return;
    }

    Campeonato.Inserir(new Dictionary<string, object>()
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