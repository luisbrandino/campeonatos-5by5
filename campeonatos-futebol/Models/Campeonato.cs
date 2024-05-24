using Microsoft.Data.SqlClient;

namespace campeonatos_futebol.Models
{
    internal class Campeonato : Model
    {
        protected static string tabela = "tb_time";

        protected static string[] colunas =
        {
            "nome"
        };

        public static new void Inserir(Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = new SqlConnection(endereco))
            {
                conexao.Open();

                using (SqlCommand comando = new SqlCommand("criar_campeonato", conexao))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (string coluna in colunas)
                        if (dados.ContainsKey(coluna))
                            comando.Parameters.AddWithValue($"@{coluna}", dados[coluna] ?? null);

                    comando.ExecuteNonQuery();
                }
            }
        }

        public static bool Existe(string nome)
        {
            if (nome.Length > 30)
                return false;

            using (SqlConnection conexao = new SqlConnection(endereco))
            {
                conexao.Open();

                using (SqlCommand comando = new SqlCommand("campeonato_existe", conexao))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    comando.Parameters.AddWithValue("@nome", nome);

                    int result = (int) comando.ExecuteScalar();

                    return result > 0;
                }
            }
        }

    }
}
