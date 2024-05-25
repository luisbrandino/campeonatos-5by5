using Microsoft.Data.SqlClient;

namespace campeonatos_futebol.Models
{
    internal abstract class Model
    {
        public abstract string Tabela { get; protected set; }

        public abstract string[] Colunas { get; protected set; }

        protected static string endereco = "Server=127.0.0.1;Database=db_futebol;User Id=sa;Password=SqlServer2019!;TrustServerCertificate=True";

        protected SqlConnection conexaoAtual;

        private SqlCommand? Procedure(string nome, Dictionary<string, object> dados)
        {
            AbrirConexao();

            SqlCommand comando = new SqlCommand(nome, conexaoAtual);

            comando.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (string coluna in Colunas)
                if (dados.ContainsKey(coluna))
                    comando.Parameters.AddWithValue(coluna, dados[coluna]);

            return comando;
        }

        protected void ProcedureNonQuery(string nome, Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = conexaoAtual)
                using (SqlCommand procedure = Procedure(nome, dados))
                    procedure.ExecuteNonQuery();
        }

        protected T ProcedureScalar<T>(string nome, Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = conexaoAtual)
                using (SqlCommand procedure = Procedure(nome, dados))
                    return (T)procedure.ExecuteScalar();
        }

        protected void AbrirConexao()
        {
            conexaoAtual?.Dispose();

            conexaoAtual = new SqlConnection(endereco);

            conexaoAtual.Open();
        }

        public virtual void Inserir(Dictionary<string, object> dados)
        {
        }

        public virtual void Buscar()
        {

        }

        public virtual void BuscarTodos()
        {

        }

        public virtual void Salvar()
        {

        }

        public virtual void Deletar()
        {

        }
    }
}
