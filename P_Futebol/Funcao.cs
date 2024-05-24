﻿using Microsoft.Data.SqlClient;
using System.Data;
namespace P_Futebol
{
    internal class Funcao
    {
        private static Banco _dbSQL = new Banco();
        private SqlConnection _connSQL = new SqlConnection(_dbSQL.Caminho());

        public Funcao()
        {

        }

        public void InserirClube()
        {
            string nome, apelido;
            DateOnly dtcriacao;
            if (RetornarClubes() == 5)
            {
                Console.WriteLine("Número de times cadastrados excedido!");
            }
            else
            {
                Console.WriteLine("Informe o nome do Time: ");
                nome = Console.ReadLine();
                Console.WriteLine("Informe o Apelido do Time: ");
                apelido = Console.ReadLine();
                Console.WriteLine("Informe a data de fundação do Time: ");
                dtcriacao = returnDate();

                Clube timeObj = new(nome, apelido, dtcriacao);
                timeObj.InserirClube(_connSQL);
            }
            Console.ReadKey();
        }

        public void ListarClubes()
        {
            string nome = "", apelido = "";
            DateOnly dtcriacao;
            try
            {
                _connSQL.Open();
                SqlCommand cmdSQL = new SqlCommand();
                cmdSQL.CommandText = " SELECT Nome, Apelido, CONVERT(VARCHAR(10),DtCriacao,103) FROM Clube ORDER BY Id";
                cmdSQL.Connection = _connSQL;
                using (SqlDataReader dr = cmdSQL.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        nome = dr[0].ToString();
                        apelido = dr[1].ToString();
                        dtcriacao = DateOnly.Parse(dr[2].ToString());
                        Console.WriteLine($"Time: {nome} Apelido: {apelido} Data Fundação: {dtcriacao}");
                    }
                }
                if (nome == "")
                {
                    Console.WriteLine("Nenhum time encontrado!");
                }
                _connSQL.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("\nPressione qualquer tecla para continuar");
            Console.ReadKey();
        }

        public void RealizarPartidas()
        {
            int countClubes = RetornarClubes();
            int countPartidas = RetornarPartidas();
            int[] listaClubes = new int[countClubes];
            int Rodada = 0;

            if (countClubes == 0)
            {
                Console.WriteLine("Não há times cadastrados!");
            }
            else if (countClubes == countPartidas)
            {
                Console.WriteLine("Todas as partidas já foram realizadas!");
            }
            else
            {
                Clube objClube = new Clube();
                listaClubes = objClube.PreencherVetorClubes(_connSQL, countClubes);
                for (int i = 0; i < countClubes; i++)
                {
                    Rodada++;
                    for (int j = 0; j < countClubes; j++)
                    {
                        if (i != j)
                        {
                            Partida objPartida = new Partida(listaClubes[i], new Random().Next(0, 10), listaClubes[j], new Random().Next(0, 10), Rodada);
                            objPartida.InserirPartida(_connSQL);
                        }
                    }
                }
                Console.WriteLine("Partidas cadastradas com sucesso!");
                Console.WriteLine("\nPressione qualquer tecla para continuar");
                Console.ReadKey();
            }
        }
        public void RetornarTabela()
        {
            int contador = 0;
            string campeao = "";

            if (RetornarPartidas() > 0)
            {
                #region SQL
                _connSQL.Open();
                SqlCommand cmdSQL = new SqlCommand();

                cmdSQL.CommandText = " SELECT (B.Nome + ' (' + B.Apelido + ') ' + CONVERT(VARCHAR(10),B.DtCriacao,103)), A.Pontuacao, A.Vitorias, A.Empates, A.Derrotas " +
                                     " FROM CLASSIFICACAO A JOIN CLUBE B\r\nON A.IdTime = B.Id " +
                                     " ORDER BY PONTUACAO DESC ";

                cmdSQL.Connection = _connSQL;
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
                _connSQL.Close();
                #endregion
            }
            else
            {
                Console.WriteLine("Não há partidas registradas!");
                Console.WriteLine("\nPressione qualquer tecla para continuar");
                Console.ReadKey();
            }
        }

        public void RetornarGols(int tipo)
        {
            if (RetornarPartidas() > 0)
            {
                #region SQL
                _connSQL.Open();
                SqlCommand cmdSQL = new SqlCommand();

                cmdSQL.Connection = _connSQL;
                switch (tipo)
                {
                    case 0: // gols realizados
                        cmdSQL.CommandText = " SELECT (B.Nome + ' (' + B.Apelido + ') ' + CONVERT(VARCHAR(10),B.DtCriacao,103)), A.GolsFeitos " +
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
                        cmdSQL.CommandText = " SELECT (B.Nome + ' (' + B.Apelido + ') ' + CONVERT(VARCHAR(10),B.DtCriacao,103)), A.GolsSofridos " +
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
                        cmdSQL.CommandText = " SELECT A.IdTime, (B.Nome + ' (' + B.Apelido + ') ' + CONVERT(VARCHAR(10),B.DtCriacao,103)), A.MaiorNroGols, " +
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
                    case 4:
                        cmdSQL.CommandText = " SELECT IdPartida, " +
                                             " (SELECT Nome + ' (' + Apelido + ') ' FROM Clube WHERE Clube.Id = Partida.IdTimeCasa), " +
                                             " CAST(GolsTimeCasa AS VARCHAR(2)), " +
                                             " CAST(GolsTimeVisitante AS VARCHAR(2)), " +
                                             " (SELECT Nome + ' (' + Apelido + ') ' FROM Clube WHERE Clube.Id = Partida.IdTimeVisitante) " +
                                             " FROM Partida";

                        Console.Write($"\n[Partida] ");
                        Console.Write($"[".PadRight(10) + "Time Casa".PadRight(21) + "] ");
                        Console.Write($"[ Jogos ]".PadRight(10));
                        Console.Write($"[".PadRight(10) + "Time Visitante".PadRight(21) + "] ");
                        using (SqlDataReader dr = cmdSQL.ExecuteReader())
                        {
                            while (dr.Read())
                            {

                                Console.Write($"\n[   {dr[0].ToString().PadRight(2)}  ]");
                                Console.Write($" [{dr[1].ToString().PadRight(30)}]");
                                Console.Write($" ({dr[2].ToString().PadRight(2)} X {dr[3].ToString().PadRight(2)})");
                                Console.Write($" [{dr[4].ToString().PadRight(30)}]");
                            }
                            Console.WriteLine("\nPressione qualquer tecla para continuar");
                            Console.ReadKey();
                        }
                        break;

                }
                _connSQL.Close();
                #endregion
            }
            else
            {
                Console.WriteLine("Não há partidas registradas!");
                Console.WriteLine("\nPressione qualquer tecla para continuar");
                Console.ReadKey();
            }
        }

        public void ResetarCampeonato()
        {
            string opc = "";
            Console.WriteLine("Deseja resetar o campeonato?\nS - Sim ... Outra tecla - Não");
            opc = Console.ReadLine();
            if (opc.ToLower() == "s")
            {
                try
                {
                    _connSQL.Open();
                    SqlCommand sql_cmnd = new SqlCommand("ResetarCampeonato", _connSQL);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.ExecuteNonQuery();
                    _connSQL.Close();
                    Console.WriteLine("Campeonato restado com sucesso!");
                    Console.WriteLine("\nPressione qualquer tecla para continuar");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void ExcluirTimes()
        {
            string opc = "";
            Console.WriteLine("Deseja resetar os times e o campeonato?\nS - Sim ... Outra tecla - Não");
            opc = Console.ReadLine();
            if (opc.ToLower() == "s")
            {
                try
                {
                    _connSQL.Open();
                    SqlCommand sql_cmnd = new SqlCommand("ResetarTimes", _connSQL);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.ExecuteNonQuery();
                    _connSQL.Close();
                    Console.WriteLine("Campeonato restado e times excluídos com sucesso!");
                    Console.WriteLine("\nPressione qualquer tecla para continuar");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public int RetornarPartidas()
        {
            int partidas = 0;
            try
            {
                _connSQL.Open();
                SqlCommand cmdSQL = new SqlCommand();
                cmdSQL.CommandText = " SELECT ISNULL((SELECT MAX(RODADA) FROM PARTIDA),0) ";
                cmdSQL.Connection = _connSQL;
                using (SqlDataReader dr = cmdSQL.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        partidas = int.Parse(dr[0].ToString());
                    }
                }
                _connSQL.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return partidas;
        }

        public int RetornarClubes()
        {
            int clubes = 0;
            try
            {
                _connSQL.Open();
                SqlCommand cmdSQL = new SqlCommand();
                cmdSQL.CommandText = " SELECT ISNULL(COUNT (*),0) FROM CLUBE ";
                cmdSQL.Connection = _connSQL;
                using (SqlDataReader dr = cmdSQL.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        clubes = int.Parse(dr[0].ToString());
                    }
                }
                _connSQL.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return clubes;
        }

        public int returnInt()
        {
            int Inteiro = 0;
            bool valor = false;

            while (!valor)
            {
                if (int.TryParse(Console.ReadLine(), out int varint))
                {
                    Inteiro = varint;
                    valor = true;
                }
                else
                {
                    Console.WriteLine("Formato inválido. Informe números inteiros apenas.");
                }
            }
            return Inteiro;
        }
        public DateOnly returnDate()
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