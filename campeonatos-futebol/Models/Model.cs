using Microsoft.Data.SqlClient;

namespace campeonatos_futebol.Models
{
    internal abstract class Model
    {
        protected string tabela;
        protected static string[] colunas;

        protected static string endereco = "Server=127.0.0.1;Database=db_futebol;User Id=sa;Password=SqlServer2019!;TrustServerCertificate=True";

        // deixando preparado pro futuro

        public static void Inserir(Dictionary<string, object> dados)
        {
        }

        public static void Buscar()
        {

        }

        public static void BuscarTodos()
        {

        }

        public void Salvar()
        {

        }

        public void Deletar()
        {

        }
    }
}
