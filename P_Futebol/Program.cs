using P_Futebol;

internal class Program
{
    private static void Main(string[] args)
    {
        Menu();
        //SqlConnection conexao = new SqlConnection();
    }
    static void Menu()
    {
        int opt = -1;
        do
        {
            Console.Clear();
            Console.WriteLine(">> FUTEBOL <<");
            Console.WriteLine("Escolha a opção desejada:");
            Console.WriteLine("1 - Cadastrar Time");
            Console.WriteLine("2 - Listar Times");
            Console.WriteLine("3 - Realizar Partidas");
            Console.WriteLine("4 - Mostrar Tabela de Classificação");
            Console.WriteLine("5 - Mostrar o time que mais fez gols");
            Console.WriteLine("6 - Mostrar o time que mais levou gols");
            Console.WriteLine("7 - Mostrar a partida que mais teve gols");
            Console.WriteLine("8 - Mostrar maior número de gols por time em uma partida");
            Console.WriteLine("9 - Mostrar todas as Partidas");
            Console.WriteLine("10 - Resetar Campeonato");
            Console.WriteLine("11 - Excluir Times e Resetar Campeonato");
            Console.WriteLine("0 - Sair do programa");
            opt = new BancoDML().returnInt();

            switch (opt)
            {
                case 0:
                    Console.WriteLine("Encerrando...");
                    break;
                case 1:
                    new BancoDML().InserirClube();
                    break;
                case 2:
                    new BancoDML().ListarClubes();
                    break;
                case 3:
                    new BancoDML().RealizarPartidas();
                    break;
                case 4:
                    new BancoDML().RetornarTabela();
                    break;
                case 5:
                    new BancoDML().RetornarGols(0);
                    break;
                case 6:
                    new BancoDML().RetornarGols(1);
                    break;
                case 7:
                    new BancoDML().RetornarGols(2);
                    break;
                case 8:
                    new BancoDML().RetornarGols(3);
                    break;
                case 9:
                    new BancoDML().RetornarGols(4);
                    break;
                case 10:
                    new BancoDML().ResetarCampeonato();
                    break;
                case 11:
                    new BancoDML().ExcluirTimes();
                    break;
                default:
                    Console.WriteLine("Opção inválida.\nPressione qualquer tecla para continuar.");
                    Console.ReadKey();
                    break;
            }
        } while (opt != 0);
    }
}