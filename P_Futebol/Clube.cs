using Microsoft.Data.SqlClient;
using System.Data;

namespace P_Futebol
{
    internal class Clube
    {
        public string nome { get; set; }
        public string apelido { get; set; }
        public DateOnly dtcriacao { get; set; }

        public Clube()
        {
            
        }
        public Clube(string nome, string apelido, DateOnly dtcriacao)
        {
            this.nome = nome;
            this.apelido = apelido;
            this.dtcriacao = dtcriacao;
        }

        public void InserirClube(SqlConnection _connSQL)
        {
            try
            {
                _connSQL.Open();
                SqlCommand comando = new("Inserir_Time", _connSQL);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add("@Nome", SqlDbType.VarChar, 30).Value = nome;
                comando.Parameters.Add("@Apelido", SqlDbType.VarChar, 30).Value = apelido;
                comando.Parameters.Add("@DtCriacao", SqlDbType.Date).Value = dtcriacao;
                comando.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Time cadastrado com sucesso.");
                _connSQL.Close();
            }
        }

        public int[] PreencherVetorClubes(SqlConnection _connSQL, int countClubes)
        {
            int contador = 0;
            int[] listaClubes = new int[countClubes];
            try
            {
                _connSQL.Open();
                SqlCommand cmdSQL = new SqlCommand();
                cmdSQL.CommandText = " SELECT Id FROM Clube ORDER BY Id";
                cmdSQL.Connection = _connSQL;
                using (SqlDataReader dr = cmdSQL.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        listaClubes[contador] = int.Parse(dr[0].ToString());
                        contador++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                _connSQL.Close();
            }
            return listaClubes;
        }
    }
}
