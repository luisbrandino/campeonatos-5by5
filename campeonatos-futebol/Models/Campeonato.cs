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
           
        }

        public bool Existe(string nome)
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
