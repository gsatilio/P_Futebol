using Microsoft.Data.SqlClient;
using System.Data;

namespace P_Futebol
{
    internal class Partida
    {
        int idTimeCasa { get; set; }
        int GolsCasa { get; set; }
        int idTimeVisitante { get; set; }
        int GolsVisitante { get; set; }
        int Rodada { get; set; }
        public Partida(int idTimeCasa, int golsCasa, int idTimeVisitante, int golsVisitante, int rodada)
        {
            this.idTimeCasa = idTimeCasa;
            GolsCasa = golsCasa;
            this.idTimeVisitante = idTimeVisitante;
            GolsVisitante = golsVisitante;
            Rodada = rodada;
        }
        public void InserirPartida(SqlConnection _connSQL)
        {
            try
            {
                _connSQL.Open();
                SqlCommand sql_cmnd = new SqlCommand("Inserir_Partida", _connSQL);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@IdTimeCasa", SqlDbType.Int).Value = idTimeCasa;
                sql_cmnd.Parameters.AddWithValue("@GolsCasa", SqlDbType.Int).Value = GolsCasa;
                sql_cmnd.Parameters.AddWithValue("@IdTimeVisitante", SqlDbType.Int).Value = idTimeVisitante;
                sql_cmnd.Parameters.AddWithValue("@GolsVisitante", SqlDbType.Int).Value = GolsVisitante;
                sql_cmnd.Parameters.AddWithValue("@Rodada", SqlDbType.Int).Value = Rodada;
                sql_cmnd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _connSQL.Close();
            }
        }
    }
}
