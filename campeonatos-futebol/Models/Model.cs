using Microsoft.Data.SqlClient;
using System.Data;

namespace campeonatos_futebol.Models
{
    internal abstract class Model
    {
        public abstract string Tabela { get; protected set; }

        public abstract string[] Colunas { get; protected set; }

        protected static string endereco = "Server=127.0.0.1;Database=db_futebol;User Id=sa;Password=SqlServer2019!;TrustServerCertificate=True";

        protected SqlConnection conexaoAtual;

        private SqlCommand? Command(string query, Dictionary<string, object>? dados = null)
        {
            AbrirConexao();

            SqlCommand comando = new SqlCommand(query, conexaoAtual);

            if (dados != null)
                foreach (var dado in dados)
                        comando.Parameters.AddWithValue(dado.Key, dado.Value);

            return comando;
        }

        protected DataRowCollection? CommandDataTable(SqlCommand comando)
        {
            using (SqlConnection conexao = conexaoAtual)
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    DataTable resultado = new();

                    adaptador.Fill(resultado);

                    if (resultado.Rows.Count <= 0)
                        return null;

                    return resultado.Rows;
                }
        }

        protected void CommandNonQuery(SqlCommand comando)
        {
            using (SqlConnection conexao = conexaoAtual)
                comando.ExecuteNonQuery();
        }

        protected T? CommandScalar<T>(SqlCommand comando)
        {
            using (SqlConnection conexao = conexaoAtual)
            {
                object resultado = comando.ExecuteScalar();

                if (resultado == null)
                    return default(T);

                return (T)resultado;
            }
        }

        private SqlCommand? Procedure(string nome, Dictionary<string, object> dados)
        {
            SqlCommand? comando = Command(nome, dados);

            if (comando == null)
                return null;

            comando.CommandType = CommandType.StoredProcedure;

            return comando;
        }

        protected void ProcedureNonQuery(string nome, Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = conexaoAtual)
            using (SqlCommand? procedure = Procedure(nome, dados))
                CommandNonQuery(procedure);
        }

        protected T? ProcedureScalar<T>(string nome, Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = conexaoAtual)
            using (SqlCommand? procedure = Procedure(nome, dados))
                return CommandScalar<T>(procedure);
        }

        protected DataRowCollection? ProcedureDataTable(string nome, Dictionary<string, object> dados)
        {
            using (SqlConnection conexao = conexaoAtual)
            using (SqlCommand? procedure = Procedure(nome, dados))
                return CommandDataTable(procedure);
        }

        protected void AbrirConexao()
        {
            conexaoAtual?.Dispose();

            conexaoAtual = new SqlConnection(endereco);

            conexaoAtual.Open();
        }

        public virtual DataRowCollection? Buscar(int id)
        {
            if (!Colunas.Contains("id"))
                return null;

            return Buscar(new Dictionary<string, object> { { "id", id } });
        }

        public virtual DataRowCollection? Buscar(Dictionary<string, object> dados)
        {
            string query = $"select {string.Join(',', Colunas.ToArray())} from {Tabela} ";

            string where = string.Join(" and ", dados.Keys);

            foreach (string key in dados.Keys)
                if (Colunas.Contains(key))
                    where = where.Replace(key, $"{key} = @{key}");

            query += "where " + where;

            return CommandDataTable(Command(query, dados));
        }

        public virtual DataRowCollection? BuscarTodos()
        {
            string query = $"select {string.Join(',', Colunas.ToArray())} from {Tabela}";

            return CommandDataTable(Command(query));
        }

        public virtual int Contagem()
        {
            return CommandScalar<int>(Command($"select count(1) from {Tabela}"));
        }
    }
}
