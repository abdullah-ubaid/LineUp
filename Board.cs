using System;
using System.Collections.Generic;
using System.Linq;

namespace LineUpGame
{
    public class Board
    {
        private int rows;
        private int cols;
        private DiscType[,] grid;

        public Board(int rows = 6, int cols = 7)
        {
            this.rows = rows;
            this.cols = cols;
            grid = new DiscType[rows, cols];
            Clear();
        }

        public void Clear()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    grid[r, c] = DiscType.Empty;
        }

        public bool DropDisc(int col, DiscType disc)
        {
            if (col < 0 || col >= cols) return false;
            for (int r = rows - 1; r >= 0; r--)
            {
                if (grid[r, col] == DiscType.Empty)
                {
                    grid[r, col] = disc;
                    return true;
                }
            }
            return false;
        }

        public bool IsFull()
        {
            for (int c = 0; c < cols; c++)
                if (grid[0, c] == DiscType.Empty)
                    return false;
            return true;
        }

        public void Display()
        {
            Console.Clear();
            for (int r = 0; r < rows; r++)
            {
                Console.Write("|");
                for (int c = 0; c < cols; c++)
                {
                    char symbol = grid[r, c] switch
                    {
                        DiscType.Player1 => 'X',
                        DiscType.Player2 => 'O',
                        _ => ' '
                    };
                    Console.Write($" {symbol} ");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine("  0  1  2  3  4  5  6");
        }

        public bool CheckWin(DiscType disc)
        {
            // Horizontal
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols - 3; c++)
                    if (Enumerable.Range(0, 4).All(i => grid[r, c + i] == disc))
                        return true;

            // Vertical
            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows - 3; r++)
                    if (Enumerable.Range(0, 4).All(i => grid[r + i, c] == disc))
                        return true;

            // Diagonal (\)
            for (int r = 0; r < rows - 3; r++)
                for (int c = 0; c < cols - 3; c++)
                    if (Enumerable.Range(0, 4).All(i => grid[r + i, c + i] == disc))
                        return true;

            // Diagonal (/)
            for (int r = 3; r < rows; r++)
                for (int c = 0; c < cols - 3; c++)
                    if (Enumerable.Range(0, 4).All(i => grid[r - i, c + i] == disc))
                        return true;

            return false;
        }

        // --- Spin mode functions ---
        public void Rotate90Clockwise()
        {
            DiscType[,] rotated = new DiscType[cols, rows];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    rotated[c, rows - 1 - r] = grid[r, c];
            grid = rotated;
            int temp = rows;
            rows = cols;
            cols = temp;
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            for (int c = 0; c < cols; c++)
            {
                List<DiscType> discs = new();
                for (int r = rows - 1; r >= 0; r--)
                    if (grid[r, c] != DiscType.Empty)
                        discs.Add(grid[r, c]);

                for (int r = rows - 1; r >= 0; r--)
                    grid[r, c] = (rows - 1 - r < discs.Count)
                                 ? discs[rows - 1 - r]
                                 : DiscType.Empty;
            }
        }
    }
}
