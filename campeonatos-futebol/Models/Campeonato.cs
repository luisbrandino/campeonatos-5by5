using Microsoft.Data.SqlClient;

namespace campeonatos_futebol.Models
{
    internal class Campeonato : Model
    {
        public override string Tabela { get; protected set; } = "tb_campeonato";

        public override string[] Colunas { get; protected set; } = new string[]
        {
            "nome"
        };

        public new void Inserir(Dictionary<string, object> dados)
        {
            ProcedureNonQuery("criar_campeonato", dados);
        }

        public bool Existe(string nome)
        {
            if (nome.Length > 30)
                return false;

            return Existe(new Dictionary<string, object> { { "nome", nome } });
        }

        public bool Existe(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("campeonato_existe", dados) > 0;
        }

    }
}
