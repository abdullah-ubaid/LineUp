using System.Text.Json;

namespace LineUpGame
{
    enum DiscType { Ordinary, Boring, Magnetic }
    enum PlayerType { Human, Computer }

    class Player
    {
        public string Name { get; set; }
        public char OrdinarySymbol { get; set; }
        //public char BoringSymbol { get; set; }
        //public char MagneticSymbol { get; set; }
        public PlayerType Type { get; set; }

        //public int BoringCount { get; set; } = 2;
        //public int MagneticCount { get; set; } = 2;

        public Player(string name, char ordinary,/* char boring, char magnetic,*/ PlayerType type)
        {
            Name = name;
            OrdinarySymbol = ordinary;
            //BoringSymbol = boring;
            //MagneticSymbol = magnetic;
            Type = type;
        }
    }

    class Game
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public char[,] Grid { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int CurrentPlayerIndex { get; set; } = 0;
        public int DiscsToWin { get; set; }

        // 新增 Undo/Redo 堆疊
        private Stack<GameState> undoStack = new();

        private Stack<GameState> redoStack = new();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        // 狀態快照
        private record GameState(char[,] Grid, int CurrentPlayerIndex);
        public Player CurrentPlayer => CurrentPlayerIndex % 2 == 0 ? Player1 : Player2;

        public Game(int rows, int cols, Player p1, Player p2)
        {
            Rows = rows;
            Cols = cols;
            Grid = new char[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Grid[r, c] = ' ';
            Player1 = p1;
            Player2 = p2;
            DiscsToWin = Math.Min(4, Math.Min(Rows, Cols));
        }
        private char[,] CloneGrid()
        {
            var clone = new char[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    clone[r, c] = Grid[r, c];
            return clone;
        }

        public void DisplayGrid()
        {
            for (int r = Rows - 1; r >= 0; r--)
            {
                Console.Write("|");
                for (int c = 0; c < Cols; c++)
                {
                    Console.Write($" {Grid[r, c]} |");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool IsValidMove(int col) => col >= 0 && col < Cols && Grid[Rows - 1, col] == ' ';

        public int MakeMove(int col, DiscType discType)
        {
            if (!IsValidMove(col)) return -1; 
            SaveState();
            for (int r = 0; r < Rows; r++)
            {
                if (Grid[r, col] == ' ')
                {
                    char symbol = discType switch
                    {
                        DiscType.Ordinary => CurrentPlayer.OrdinarySymbol,
                        //DiscType.Boring => CurrentPlayer.BoringSymbol,
                        //DiscType.Magnetic => CurrentPlayer.MagneticSymbol,
                        _ => ' '
                    };

                    Grid[r, col] = symbol;

                    // Trigger Boring effect
                    //if (discType == DiscType.Boring)
                    //    ApplyBoringEffect(col, r);

                    // Trigger Magnetic effect
                    //if (discType == DiscType.Magnetic)
                    //    ApplyMagneticEffect(col, r);

                    return r;
                }
            }
            return -1;
        }

        //private void ApplyBoringEffect(int col, int row)
        //{
        //    // Remove all discs below boring disc
        //    for (int r = 0; r < row; r++)
        //    {
        //        char c = Grid[r, col];
        //        if (c != ' ')
        //        {
        //            Grid[r, col] = ' ';
        //        }
        //    }
        //}

        //private void ApplyMagneticEffect(int col, int row)
        //{
        //    for (int r = row - 1; r >= 0; r--)
        //    {
        //        char c = Grid[r, col];
        //        if (c == CurrentPlayer.OrdinarySymbol)
        //        {
        //            if (r + 1 < Rows && Grid[r + 1, col] == CurrentPlayer.MagneticSymbol)
        //                return;
        //            Grid[r + 1, col] = c;
        //            Grid[r, col] = ' ';
        //            return;
        //        }
        //    }
        //}

        public bool CheckWin(Player player)
        {
            char sym = player.OrdinarySymbol;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] != sym) continue;

                    if (CheckDirection(r, c, 0, 1, sym)) return true;
                    if (CheckDirection(r, c, 1, 0, sym)) return true;
                    if (CheckDirection(r, c, 1, 1, sym)) return true;
                    if (CheckDirection(r, c, 1, -1, sym)) return true;
                }
            }
            return false;
        }

        private bool CheckDirection(int r, int c, int dr, int dc, char sym)
        {
            int count = 0;
            for (int i = 0; i < DiscsToWin; i++)
            {
                int nr = r + dr * i;
                int nc = c + dc * i;
                if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols && Grid[nr, nc] == sym)
                    count++;
                else
                    break;
            }
            return count == DiscsToWin;
        }

        public bool IsFull()
        {
            for (int c = 0; c < Cols; c++)
                if (Grid[Rows - 1, c] == ' ')
                    return false;
            return true;
        }

        public void SwitchPlayer() => CurrentPlayerIndex = 1 - CurrentPlayerIndex;

        public void Save(string path)
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            Console.WriteLine("Game saved.");
            string? load = Console.ReadLine();
            if (string.IsNullOrEmpty(load))
            {
                load = "n"; // Default to "n" if input is null or empty
            }
        }
        public void SaveState()
        {
            undoStack.Push(new GameState(CloneGrid(), CurrentPlayerIndex));
            redoStack.Clear();
        }
        public void Undo()
        {
            if (!CanUndo) return;
            redoStack.Push(new GameState(CloneGrid(), CurrentPlayerIndex));
            var prev = undoStack.Pop();
            Array.Copy(prev.Grid, Grid, Grid.Length);
            CurrentPlayerIndex = prev.CurrentPlayerIndex;
        }

        public void Redo()
        {
            if (!CanRedo) return;
            undoStack.Push(new GameState(CloneGrid(), CurrentPlayerIndex));
            var next = redoStack.Pop();
            Array.Copy(next.Grid, Grid, Grid.Length);
            CurrentPlayerIndex = next.CurrentPlayerIndex;
        }

        public static Game Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file '{path}' does not exist.");
            }
            var json = File.ReadAllText(path);
            var game = JsonSerializer.Deserialize<Game>(json);
            if (game == null)
            {
                throw new InvalidOperationException("Failed to load the game. The deserialized object is null.");
            }
            game.SaveState();

            return game;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Welcome to LineUp!");
            Game? game = null;

            Console.Write("Load saved game? (y/n): ");
            string? load = Console.ReadLine();
            if (string.IsNullOrEmpty(load))
            {
                load = "n"; // Default to "n" if input is null or empty
            }
            if (load.ToLower() == "y")
            {
                Console.Write("Enter file path: ");
                string? path = Console.ReadLine();
                if (string.IsNullOrEmpty(path))
                {
                    throw new InvalidOperationException("Path cannot be null or empty.");
                }
                game = Game.Load(path);
            }
            else
            {
                game = SetupNewGame();
            }

            while (true)
            {
                game.DisplayGrid();
                var player = game.CurrentPlayer;

                Console.WriteLine($"{player.Name}'s turn. (O=Ordinary, B=Boring, M=Magnetic, S=Save, H=Help)");
                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                if (input.ToUpper() == "S")
                {
                    Console.Write("Enter file name to save: ");
                    string? file = Console.ReadLine();
                    if (string.IsNullOrEmpty(file))
                    {
                        throw new InvalidOperationException("File name cannot be null or empty.");
                    }
                    game.Save(file);
                    continue;
                }
                if (input.ToUpper() == "H")
                {
                    ShowHelp();
                    continue;
                }
                if (input.ToUpper() == "U")
                {
                    if (game.CanUndo)
                    {
                        game.Undo();
                        Console.WriteLine("Undo successful.");
                    }
                    else
                    {
                        Console.WriteLine("Cannot undo.");
                    }
                    continue;
                }
                if (input.ToUpper() == "R")
                {
                    if (game.CanRedo)
                    {
                        game.Redo();
                        Console.WriteLine("Redo successful.");
                    }
                    else
                    {
                        Console.WriteLine("Cannot redo.");
                    }
                    continue;
                }
                // Parse input like O4, B3
                DiscType disc = DiscType.Ordinary;
                int col = -1;
                try
                {
                    char discChar = char.ToUpper(input[0]);
                    disc = discChar switch
                    {
                        'O' => DiscType.Ordinary,
                        'B' => DiscType.Boring,
                        'M' => DiscType.Magnetic,
                        _ => DiscType.Ordinary
                    };
                    col = int.Parse(input.Substring(1)) - 1;
                }
                catch
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (!game.IsValidMove(col))
                {
                    Console.WriteLine("Invalid move. Try again.");
                    continue;
                }

                game.MakeMove(col, disc);

                if (game.CheckWin(player))
                {
                    game.DisplayGrid();
                    Console.WriteLine($"{player.Name} wins!");
                    break;
                }

                if (game.IsFull())
                {
                    game.DisplayGrid();
                    Console.WriteLine("Game ends in a tie.");
                    break;
                }

                game.SwitchPlayer();
            }
        }

        static Game SetupNewGame()
        {
            int rows;
            while (true)
            {
                Console.Write("Enter number of rows: ");
                string? rowsInput = Console.ReadLine();
                if (!int.TryParse(rowsInput, out rows) || rows < 1)
                {
                    Console.WriteLine("Invalid input. Please enter a positive integer for rows.");
                    continue;
                }
                break;
            }

            int cols;
            while (true)
            {
                Console.Write("Enter number of columns: ");
                string? colsInput = Console.ReadLine();
                if (!int.TryParse(colsInput, out cols) || cols < 1)
                {
                    Console.WriteLine("Invalid input. Please enter a positive integer for columns.");
                    continue;
                }
                break;
            }

            PlayerType p1Type;
            while (true)
            {
                Console.Write("Select Player1 type (h/c): ");
                string? p1TypeInput = Console.ReadLine()?.ToLower();
                if (p1TypeInput == "h")
                {
                    p1Type = PlayerType.Human;
                    break;
                }
                else if (p1TypeInput == "c")
                {
                    p1Type = PlayerType.Computer;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'h' for Human or 'c' for Computer.");
                }
            }
            PlayerType p2Type;
            while (true)
            {
                Console.Write("Select Player2 type (h/c): ");
                string? p2TypeInput = Console.ReadLine()?.ToLower();
                if (p2TypeInput == "h")
                {
                    p2Type = PlayerType.Human;
                    break;
                }
                else if (p2TypeInput == "c")
                {
                    p2Type = PlayerType.Computer;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'h' for Human or 'c' for Computer.");
                }
            }

            if (p1Type == PlayerType.Computer && p2Type == PlayerType.Computer)
            {
                Console.WriteLine("Computer vs. Computer is not allowed. Please select valid player types.");
                return SetupNewGame();
            }

            var p1 = new Player("Player1", '@', /*'B', 'M',*/ p1Type);
            var p2 = new Player("Player2", '#', /*'b', 'm',*/ p2Type);

            return new Game(rows, cols, p1, p2);
        }

        static void ShowHelp()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("O# -> Ordinary disc in column #");
            Console.WriteLine("B# -> Boring disc in column #");
            Console.WriteLine("M# -> Magnetic disc in column #");
            Console.WriteLine("S -> Save game");
            Console.WriteLine("H -> Show help"); 
            Console.WriteLine("R -> Redo move");
            Console.WriteLine("H -> Show help");
        }
    }
}
