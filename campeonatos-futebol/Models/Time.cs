using System.Data;

namespace campeonatos_futebol.Models
{
    internal class Time : Model
    {
        public override string Tabela { get; protected set; } = "tb_time";

        public override string[] Colunas { get; protected set; } = new string[]
        {
            "id",
            "nome",
            "apelido",
            "data_criacao"
        };

        public DataRowCollection? Buscar(string nome)
        {
            return base.Buscar(new Dictionary<string, object> { { "nome", nome } });
        }

        public int Inserir(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("criar_time", dados);
        }

        public bool Existe(string nome)
        {
            if (nome.Length > 30)
                return false;

            return Existe(new Dictionary<string, object> { { "nome", nome } });
        }

        public bool Existe(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("time_existe", dados) > 0;
        }
    }
}
