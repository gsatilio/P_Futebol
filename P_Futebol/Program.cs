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
            Console.WriteLine("2 - Realizar Partidas");
            Console.WriteLine("3 - Mostrar Tabela de Classificação");
            Console.WriteLine("4 - Mostrar o time que mais fez gols");
            Console.WriteLine("5 - Mostrar o time que mais levou gols");
            Console.WriteLine("6 - Mostrar a partida que mais teve gols");
            Console.WriteLine("7 - Mostrar maior número de gols por time em uma partida");
            Console.WriteLine("8 - Resetar Campeonato");
            Console.WriteLine("0 - Sair do programa");
            opt = returnInt();

            switch (opt)
            {
                case 0:
                    Console.WriteLine("Encerrando...");
                    break;
                case 1:
                    new BancoDML().InserirTime();
                    break;
                case 2:
                    new BancoDML().InserirPartida();
                    break;
                case 3:
                    new BancoDML().RetornarTabela();
                    break;
                case 4:
                    new BancoDML().RetornarGols(0);
                    break;
                case 5:
                    new BancoDML().RetornarGols(1);
                    break;
                case 6:
                    new BancoDML().RetornarGols(2);
                    break;
                case 7:
                    new BancoDML().RetornarGols(3);
                    break;
                case 8:
                    new BancoDML().ResetarCampeonato();
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        } while (opt != 0);
    }
    static int returnInt()
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
}