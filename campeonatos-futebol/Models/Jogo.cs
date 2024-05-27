
namespace campeonatos_futebol.Models
{
    internal class Jogo : Model
    {
        public override string Tabela { get; protected set; } = "tb_jogo";

        public override string[] Colunas { get; protected set; } = new string[]
        {
            "campeonato_id",
            "time_mandante_id",
            "time_visitante_id",
        };

        public int Inserir(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("criar_jogo", dados);
        }

        public void DefinirGolsTimeMandante(Dictionary<string, object> dados)
        {
            ProcedureNonQuery("definir_gols_time_mandante", dados);
        }

        public void DefinirGolsTimeVisitante(Dictionary<string, object> dados)
        {
            ProcedureNonQuery("definir_gols_time_visitante", dados);
        }

        public bool Existe(int campeonatoId, int timeMandanteId, int timeVisitanteId)
        {
            return Existe(new Dictionary<string, object>
            {
                { "campeonato_id", campeonatoId },
                { "time_mandante_id", timeMandanteId },
                { "time_visitante_id", timeVisitanteId }
            });
        }

        public bool Existe(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("jogo_existe", dados) > 0;
        }
    }
}
