using Microsoft.Data.SqlClient;

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
            using (SqlConnection conexao = new SqlConnection(endereco))
            {
                conexao.Open();

                using (SqlCommand comando = new SqlCommand("criar_participante", conexao))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (string coluna in Colunas)
                        if (dados.ContainsKey(coluna))
                            comando.Parameters.AddWithValue($"@{coluna}", dados[coluna] ?? null);

                    comando.ExecuteNonQuery();
                }
            }
        }
        public bool Existe(Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = new SqlConnection(endereco))
            {
                conexao.Open();

                using (SqlCommand comando = new SqlCommand("participante_existe", conexao))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (string coluna in Colunas)
                        if (dados.ContainsKey(coluna))
                            comando.Parameters.AddWithValue($"@{coluna}", dados[coluna] ?? null);

                    int result = (int)comando.ExecuteScalar();

                    return result > 0;
                }
            }
        }
    }
}
