using System;

namespace LineUpGame
{
    public class HumanPlayer : IPlayer
    {
        public string Name { get; }
        public DiscType Disc { get; }

        public HumanPlayer(string name, DiscType disc)
        {
            Name = name;
            Disc = disc;
        }

        public int ChooseColumn(Board board)
        {
            Console.Write($"{Name} (Disc: {Disc}) — Enter column (0-6): ");
            int.TryParse(Console.ReadLine(), out int col);
            return col;
        }
    }
}
