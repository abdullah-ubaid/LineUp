using System;

namespace LineUpGame
{
    public class LineUpGame
    {
        private readonly Board board;
        private readonly IPlayer[] players;
        private readonly MoveHistory history = new();
        private int turnCount = 0;

        public LineUpGame(IPlayer p1, IPlayer p2)
        {
            board = new Board();
            players = new IPlayer[] { p1, p2 };
        }

        public void StartClassic()
        {
            int current = 0;
            while (true)
            {
                board.Display();
                var player = players[current];
                Console.WriteLine("Commands: u = Undo, r = Redo, q = Quit");
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "q") break;
                if (input == "u" && history.CanUndo) { history.Undo(); continue; }
                if (input == "r" && history.CanRedo) { history.Redo(); continue; }

                if (int.TryParse(input, out int col))
                {
                    if (board.DropDisc(col, player.Disc))
                    {
                        history.RecordMove(col);
                        turnCount++;

                        if (board.CheckWin(player.Disc))
                        {
                            board.Display();
                            Console.WriteLine($"{player.Name} wins!");
                            break;
                        }

                        if (board.IsFull())
                        {
                            Console.WriteLine("Draw!");
                            break;
                        }

                        current = 1 - current;
                    }
                }
            }
        }

        public void StartSpin()
        {
            int current = 0;
            while (true)
            {
                board.Display();
                var player = players[current];
                Console.WriteLine("Commands: u = Undo, r = Redo, q = Quit");
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "q") break;

                if (int.TryParse(input, out int col))
                {
                    if (board.DropDisc(col, player.Disc))
                    {
                        history.RecordMove(col);
                        turnCount++;

                        if (turnCount % 5 == 0)
                        {
                            Console.WriteLine(">>> Board spins 90° clockwise!");
                            board.Rotate90Clockwise();
                        }

                        if (board.CheckWin(player.Disc))
                        {
                            board.Display();
                            Console.WriteLine($"{player.Name} wins!");
                            break;
                        }

                        if (board.IsFull())
                        {
                            Console.WriteLine("Draw!");
                            break;
                        }

                        current = 1 - current;
                    }
                }
            }
        }
    }
}
