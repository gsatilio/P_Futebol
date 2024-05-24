namespace P_Futebol
{
    internal class Menu
    {
        public Menu()
        {
            int opt = -1;
            do
            {
                Console.Clear();
                Console.WriteLine(">-> FUTEBOL <-<");
                Console.WriteLine("Escolha a opção desejada:");
                Console.WriteLine("01 - Cadastrar Time");
                Console.WriteLine("02 - Listar Times");
                Console.WriteLine("03 - Realizar Partidas");
                Console.WriteLine("04 - Mostrar Tabela de Classificação");
                Console.WriteLine("05 - Mostrar o time que mais fez gols");
                Console.WriteLine("06 - Mostrar o time que mais levou gols");
                Console.WriteLine("07 - Mostrar a partida que mais teve gols");
                Console.WriteLine("08 - Mostrar maior número de gols por time em uma partida");
                Console.WriteLine("09 - Mostrar todas as Partidas");
                Console.WriteLine("10 - Resetar Campeonato");
                Console.WriteLine("11 - Excluir Times e Resetar Campeonato");
                Console.WriteLine("0 - Sair do programa");
                opt = new Funcao().returnInt();

                switch (opt)
                {
                    case 0:
                        Console.WriteLine("Encerrando...");
                        break;
                    case 1:
                        new Funcao().InserirClube();
                        break;
                    case 2:
                        new Funcao().ListarClubes();
                        break;
                    case 3:
                        new Funcao().RealizarPartidas();
                        break;
                    case 4:
                        new Funcao().RetornarTabela();
                        break;
                    case 5:
                        new Funcao().RetornarGols(0);
                        break;
                    case 6:
                        new Funcao().RetornarGols(1);
                        break;
                    case 7:
                        new Funcao().RetornarGols(2);
                        break;
                    case 8:
                        new Funcao().RetornarGols(3);
                        break;
                    case 9:
                        new Funcao().RetornarGols(4);
                        break;
                    case 10:
                        new Funcao().ResetarCampeonato();
                        break;
                    case 11:
                        new Funcao().ExcluirTimes();
                        break;
                    default:
                        Console.WriteLine("Opção inválida.\nPressione qualquer tecla para continuar.");
                        Console.ReadKey();
                        break;
                }
            } while (opt != 0);
        }
    }
}
