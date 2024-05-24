using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_Futebol
{
    internal class Banco
    {
        readonly string Conexao = "Data Source=127.0.0.1; Initial Catalog=dbFutebol; User Id=SA; Password=SqlServer2019!;TrustServerCertificate=True";

        public Banco()
        {
            
        }
        public string Caminho()
        {
            return Conexao;
        }
    }
}
