using Microsoft.Data.SqlClient;

namespace campeonatos_futebol.Models
{
    internal class Participante : Model
    {
        protected static string tabela = "tb_participante";

        protected static string[] colunas =
        {
            "campeonato_id",
            "time_id",
            "pontos",
            "total_gols"
        };

        public static new void Inserir(Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = new SqlConnection(endereco))
            {
                conexao.Open();

                using (SqlCommand comando = new SqlCommand("criar_participante", conexao))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (string coluna in colunas)
                        if (dados.ContainsKey(coluna))
                            comando.Parameters.AddWithValue($"@{coluna}", dados[coluna] ?? null);

                    comando.ExecuteNonQuery();
                }
            }
        }
        public static bool Existe(Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = new SqlConnection(endereco))
            {
                conexao.Open();

                using (SqlCommand comando = new SqlCommand("participante_existe", conexao))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (string coluna in colunas)
                        if (dados.ContainsKey(coluna))
                            comando.Parameters.AddWithValue($"@{coluna}", dados[coluna] ?? null);

                    int result = (int)comando.ExecuteScalar();

                    return result > 0;
                }
            }
        }
    }
}
