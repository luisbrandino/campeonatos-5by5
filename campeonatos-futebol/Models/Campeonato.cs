using System.Data;

namespace campeonatos_futebol.Models
{
    internal class Campeonato : Model
    {
        public override string Tabela { get; protected set; } = "tb_campeonato";

        public override string[] Colunas { get; protected set; } = new string[]
        {
            "id",
            "nome",
            "time_campeao_id"
        };

        public DataRow BuscarTimeComMaisGols(int campeonatoId)
        {
            return ProcedureDataTable("buscar_time_com_mais_gols", new Dictionary<string, object> { {"campeonato_id", campeonatoId } })[0];
        }

        public DataRow BuscarTimeQueTomouMaisGols(int campeonatoId)
        {
            return ProcedureDataTable("buscar_time_que_tomou_mais_gols", new Dictionary<string, object> { { "campeonato_id", campeonatoId } })[0];
        }

        public DataRow BuscarJogoComMaisGols(int campeonatoId)
        {
            return ProcedureDataTable("buscar_jogo_com_mais_gols", new Dictionary<string, object> { { "campeonato_id", campeonatoId } })[0];
        }

        public DataRowCollection BuscarMaiorNumeroDeGolsDeCadaTime(int campeonatoId)
        {
            return ProcedureDataTable("buscar_maior_numero_de_gols_de_cada_time", new Dictionary<string, object> { { "campeonato_id", campeonatoId } });
        }

        public int Inserir(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("criar_campeonato", dados);
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
