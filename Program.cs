using System;

namespace LineUpGame
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Welcome to LineUp Game ===");
            Console.Write("Enter Player 1 name: ");
            var p1 = new HumanPlayer(Console.ReadLine() ?? "Player 1", DiscType.Player1);

            Console.Write("Enter Player 2 name: ");
            var p2 = new HumanPlayer(Console.ReadLine() ?? "Player 2", DiscType.Player2);

            LineUpGame game = new(p1, p2);

            Console.WriteLine("Select Mode: 1 = Classic, 2 = Spin");
            string mode = Console.ReadLine()?.Trim() ?? "1";

            if (mode == "2")
                game.StartSpin();
            else
                game.StartClassic();

            Console.WriteLine("Game Over — Press any key to exit.");
            Console.ReadKey();
        }
    }
}
