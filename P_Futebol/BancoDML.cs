using Microsoft.Data.SqlClient;
using System.Data;
namespace P_Futebol
{
    internal class BancoDML
    {
        private static Banco _dbSQL = new Banco();
        private SqlConnection _connSQL = new SqlConnection(_dbSQL.Caminho());

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

            #region Inserir
            _connSQL.Open();
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

            cmdSQL.Connection = _connSQL;
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
            _connSQL.Close();
            #endregion
        }

        public void InserirPartida()
        {
            int clubes = 0, partidas = 0;
            using (_connSQL)
            {
                try
                {
                    _connSQL.Open();
                    SqlCommand cmdSQL = new SqlCommand();
                    cmdSQL.CommandText = " SELECT ISNULL((SELECT MAX(RODADA) FROM PARTIDA),0), ISNULL((SELECT COUNT(*) FROM CLUBE),0) ";
                    cmdSQL.Connection = _connSQL;
                    using (SqlDataReader dr = cmdSQL.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            clubes = int.Parse(dr[0].ToString());
                            partidas = int.Parse(dr[1].ToString());
                        }
                    }
                    _connSQL.Close();

                    if (clubes == partidas)
                    {
                        Console.WriteLine("Atenção - Todas as partidas já foram registradas! Resete o campeonato para gerar novas partidas.");
                    }
                    else
                    {
                        _connSQL.Open();
                        SqlCommand sql_cmnd = new SqlCommand("CriaPartida", _connSQL);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.ExecuteNonQuery();
                        _connSQL.Close();
                        Console.WriteLine("Partidas geradas com sucesso!");
                    }
                    Console.WriteLine("\nPressione qualquer tecla para continuar");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void RetornarTabela()
        {
            int contador = 0;
            string campeao = "";

            #region Inserir
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

        public void RetornarGols(int tipo)
        {

            #region Ler
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

            }
            _connSQL.Close();
            #endregion
        }

        public void ResetarCampeonato()
        {
            using (_connSQL)
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
