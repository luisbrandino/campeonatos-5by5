namespace campeonatos_futebol.Models
{
    internal class Participante : Model
    {
        public override string Tabela { get; protected set; } = "tb_participante";

        public override string[] Colunas { get; protected set; } = new string[]
        {
            "campeonato_id",
            "time_id",
            "pontos",
            "total_gols"
        };

        public override void Inserir(Dictionary<string, object> dados)
        {
            ProcedureNonQuery("criar_participante", dados);   
        }

        public bool Existe(int campeonatoId, int timeId)
        {
            return Existe(new Dictionary<string, object> { { "campeonato_id", campeonatoId }, { "time_id", timeId } });
        }
        
        public bool Existe(Dictionary<string, object> dados)
        {
            return ProcedureScalar<int>("participante_existe", dados) > 0;
        }
    }
}
