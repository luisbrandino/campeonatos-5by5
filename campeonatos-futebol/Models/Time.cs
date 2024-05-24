using Microsoft.Data.SqlClient;

namespace campeonatos_futebol.Models
{
    internal class Time : Model
    {
        public override string Tabela { get; protected set; } = "tb_time";

        public override string[] Colunas { get; protected set; } = new string[]
        {
            "nome",
            "apelido",
            "data_criacao"
        };

        public override void Inserir(Dictionary<string, object> dados)
        {
            ProcedureNonQuery("criar_time", dados);
        }

        public bool Existe(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("time_existe", dados) > 0;
        }
    }
}
