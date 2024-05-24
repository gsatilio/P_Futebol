using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P_Futebol
{
    internal class BancoDML
    {
        public BancoDML()
        {

        }

        public void InserirTime()
        {
            string nome, apelido;
            DateOnly dtcriacao;

            Console.WriteLine("Informe o nome do Time: ");
            nome = Console.ReadLine();
            Console.WriteLine("Informe o Apelido do Time: ");
            apelido = Console.ReadLine();
            Console.WriteLine("Informe a data de criação do Time: ");
            dtcriacao = returnDate();

            Clube timeObj = new(nome, apelido, dtcriacao);

            #region Conexao com o Banco
            Banco dbSQL = new Banco();
            SqlConnection connSQL = new SqlConnection(dbSQL.Caminho());
            #endregion

            #region Inserir
            connSQL.Open();
            SqlCommand cmdSQL = new SqlCommand();

            cmdSQL.CommandText = " EXEC Inserir_Time @nome, @apelido, @dtcriacao; ";

            SqlParameter nomeSQL = new SqlParameter("@nome", System.Data.SqlDbType.VarChar, 30);
            SqlParameter apelidoSQL = new SqlParameter("@apelido", System.Data.SqlDbType.VarChar, 30);
            SqlParameter dtcriacaoSQL = new SqlParameter("@dtcriacao", System.Data.SqlDbType.Date);

            nomeSQL.Value = timeObj.nome;
            apelidoSQL.Value = timeObj.apelido;
            dtcriacaoSQL.Value = timeObj.dtcriacao;

            cmdSQL.Parameters.Add(nomeSQL);
            cmdSQL.Parameters.Add(apelidoSQL);
            cmdSQL.Parameters.Add(dtcriacaoSQL);

            cmdSQL.Connection = connSQL;
            //cmdSQL.ExecuteNonQuery();
            using (SqlDataReader dr = cmdSQL.ExecuteReader())
            {
                while (dr.Read())
                {
                    Console.WriteLine(dr[0].ToString());
                    Console.WriteLine("Pressione qualquer tecla para continuar");
                    Console.ReadKey();
                }
            }
            connSQL.Close();
            #endregion
        }
        public void RetornarTabela()
        {
            int contador = 0;
            string campeao = "";
            #region Conexao com o Banco
            Banco dbSQL = new Banco();
            SqlConnection connSQL = new SqlConnection(dbSQL.Caminho());
            #endregion

            #region Inserir
            connSQL.Open();
            SqlCommand cmdSQL = new SqlCommand();

            cmdSQL.CommandText = " SELECT B.Nome, A.Pontuacao, A.Vitorias, A.Empates, A.Derrotas " +
                                 " FROM CLASSIFICACAO A JOIN CLUBE B\r\nON A.IdTime = B.Id " +
                                 " ORDER BY PONTUACAO DESC ";

            cmdSQL.Connection = connSQL;
            using (SqlDataReader dr = cmdSQL.ExecuteReader())
            {
                while (dr.Read())
                {
                    if (contador == 0)
                    {
                        campeao = "[ ***** CAMPEÃO ***** ]";
                    }
                    Console.WriteLine($"\n[ {contador + 1} ] Time: {dr[0].ToString()} {campeao}");
                    Console.Write($"Pontuação: {dr[1].ToString()}");
                    Console.Write($"   Vitórias: {dr[2].ToString()}");
                    Console.Write($"   Empates: {dr[3].ToString()}");
                    Console.Write($"   Derrotas: {dr[4].ToString()}");
                    campeao = "";
                    contador++;
                }
                Console.WriteLine("\nPressione qualquer tecla para continuar");
                Console.ReadKey();
            }
            connSQL.Close();
            #endregion
        }

        public void RetornarGols(int tipo)
        {
            #region Conexao com o Banco
            Banco dbSQL = new Banco();
            SqlConnection connSQL = new SqlConnection(dbSQL.Caminho());
            #endregion

            #region Ler
            connSQL.Open();
            SqlCommand cmdSQL = new SqlCommand();

            cmdSQL.Connection = connSQL;
            switch (tipo)
            {
                case 0: // gols realizados
                    cmdSQL.CommandText = " SELECT B.Nome, A.GolsFeitos " +
                                         " FROM CLASSIFICACAO A JOIN CLUBE B ON A.IdTime = B.Id " +
                                         " WHERE A.GolsFeitos = (SELECT MAX(GolsFeitos) FROM CLASSIFICACAO) ";
                    using (SqlDataReader dr = cmdSQL.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Console.WriteLine($"Time: {dr[0].ToString()} - Gols Realizados: {dr[1].ToString()}");
                        }
                        Console.WriteLine("\nPressione qualquer tecla para continuar");
                        Console.ReadKey();
                    }
                    break;
                case 1: // gols sofridos
                    cmdSQL.CommandText = " SELECT B.Nome, A.GolsSofridos " +
                                         " FROM CLASSIFICACAO A JOIN CLUBE B ON A.IdTime = B.Id " +
                                         " WHERE A.GolsSofridos = (SELECT MAX(GolsSofridos) FROM CLASSIFICACAO) ";
                    using (SqlDataReader dr = cmdSQL.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Console.WriteLine($"Time: {dr[0].ToString()} - Gols Sofridos: {dr[1].ToString()}");
                        }
                        Console.WriteLine("\nPressione qualquer tecla para continuar");
                        Console.ReadKey();
                    }
                    break;
                case 2: // partida com mais gols
                    cmdSQL.CommandText = "SELECT IdPartida, " +
                                            " (SELECT C.NOME FROM CLUBE C WHERE C.ID = IdTimeCasa) TimeCasa, " +
                                            " GolsTimeCasa," +
                                            " (SELECT C.NOME FROM CLUBE C WHERE C.ID = IdTimeVisitante) TimeVisitante, " +
                                            " GolsTimeVisitante," +
                                            " (GolsTimeCasa + GolsTimeVisitante) TotalGols" +
                                            " FROM PARTIDA" +
                                            " WHERE(GolsTimeCasa + GolsTimeVisitante) = (SELECT MAX((GolsTimeCasa + GolsTimeVisitante)) FROM Partida)";
                    using (SqlDataReader dr = cmdSQL.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Console.WriteLine($"ID Partida: {dr[0].ToString()}");
                            Console.WriteLine($"Time da Casa: {dr[1].ToString()}");
                            Console.WriteLine($"Gols da Casa: {dr[2].ToString()}");
                            Console.WriteLine($"Time Visitante: {dr[3].ToString()}");
                            Console.WriteLine($"Gols do Visitante: {dr[4].ToString()}");
                            Console.WriteLine($"Total de Gols na Partida: [ {dr[5].ToString()} ]");
                        }
                        Console.WriteLine("\nPressione qualquer tecla para continuar");
                        Console.ReadKey();
                    }
                    break;
                case 3: // maior nro gols por time
                    cmdSQL.CommandText = " SELECT A.IdTime, B.Nome, A.MaiorNroGols, " +
                                         " ISNULL((SELECT TOP 1 idPartida FROM PARTIDA WHERE IdTimeCasa = IdTime AND GolsTimeCasa = MaiorNroGols), " +
                                         " (SELECT TOP 1 idPartida FROM PARTIDA WHERE IdTimeVisitante = IdTime AND GolsTimeVisitante = MaiorNroGols)) IdPartida " +
                                         " FROM CLASSIFICACAO A JOIN CLUBE B " +
                                         " ON A.IdTime = B.Id ";
                    using (SqlDataReader dr = cmdSQL.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Console.WriteLine($"\nTime: {dr[1].ToString()}");
                            Console.Write($"Maior nro de Gols em uma partida: [ {dr[2].ToString()} ]");
                            Console.Write($"   ID da Partida: {dr[3].ToString()}");
                        }
                        Console.WriteLine("\nPressione qualquer tecla para continuar");
                        Console.ReadKey();
                    }
                    break;

            }
            connSQL.Close();
            #endregion
        }


        public static DateOnly returnDate()
        {
            DateOnly Date;
            bool valor = false;

            while (!valor)
            {
                if (DateOnly.TryParse(Console.ReadLine(), out DateOnly varint))
                {
                    Date = varint;
                    valor = true;
                }
                else
                {
                    Console.WriteLine("Data inválida. Informe no formato dd/mm/aaaa.");
                }
            }
            return Date;
        }
    }
}
