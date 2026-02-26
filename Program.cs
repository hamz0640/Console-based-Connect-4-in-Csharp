using System;
using System.Media;
using System.Linq;
class ConnectFour
{
    public const int PLAYER = 1;
    public const int AI = 2;
    public const int EMPTY = 0;
    const int ROWS = 6;
    const int COLS = 7;
    const int MAX_DEPTH = 7;

    private int[,] board;

    public ConnectFour()
    {
        board = new int[ROWS, COLS];
        for (int row = 0; row < ROWS; row++)
            for (int col = 0; col < COLS; col++)
                board[row, col] = EMPTY;
    }

    public bool IsValidMove(int col)
    {
        return board[0, col] == EMPTY;
    }

    public void MakeMove(int col, int player)
    {
        for (int row = ROWS - 1; row >= 0; row--)
        {
            if (board[row, col] == EMPTY)
            {
                board[row, col] = player;
                break;
            }
        }
    }

    public void UndoMove(int col)
    {
        for (int row = 0; row < ROWS; row++)
        {
            if (board[row, col] != EMPTY)
            {
                board[row, col] = EMPTY;
                break;
            }
        }
    }

    public bool CheckWin(int player)
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col <= COLS - 4; col++)
            {
                if (board[row, col] == player && board[row, col + 1] == player &&
                    board[row, col + 2] == player && board[row, col + 3] == player)
                {
                    return true;
                }
            }
        }

        for (int row = 0; row <= ROWS - 4; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                if (board[row, col] == player && board[row + 1, col] == player &&
                    board[row + 2, col] == player && board[row + 3, col] == player)
                {
                    return true;
                }
            }
        }

        for (int row = 0; row <= ROWS - 4; row++)
        {
            for (int col = 0; col <= COLS - 4; col++)
            {
                if (board[row, col] == player && board[row + 1, col + 1] == player &&
                    board[row + 2, col + 2] == player && board[row + 3, col + 3] == player)
                {
                    return true;
                }
            }
        }

        for (int row = 3; row < ROWS; row++)
        {
            for (int col = 0; col <= COLS - 4; col++)
            {
                if (board[row, col] == player && board[row - 1, col + 1] == player &&
                    board[row - 2, col + 2] == player && board[row - 3, col + 3] == player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsBoardFull()
    {
        for (int col = 0; col < COLS; col++)
        {
            if (IsValidMove(col)) return false;
        }
        return true;
    }

    private int EvaluateBoard()
    {
        int score = 0;

        int centerCol = COLS / 2;
        for (int row = 0; row < ROWS; row++)
        {
            if (board[row, centerCol] == AI) score += 3;
            else if (board[row, centerCol] == PLAYER) score -= 3;
        }

        score += EvaluatePotentialWins(AI) - EvaluatePotentialWins(PLAYER);

        return score;
    }

    private int EvaluatePotentialWins(int player)
    {
        int potentialWins = 0;

        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col <= COLS - 4; col++)
            {
                int[] window = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    window[i] = board[row, col + i];
                }
                potentialWins += EvaluateWindow(window, player);
            }
        }

        for (int row = 0; row <= ROWS - 4; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                int[] window = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    window[i] = board[row + i, col];
                }
                potentialWins += EvaluateWindow(window, player);
            }
        }

        for (int row = 0; row <= ROWS - 4; row++)
        {
            for (int col = 0; col <= COLS - 4; col++)
            {
                int[] window = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    window[i] = board[row + i, col + i];
                }
                potentialWins += EvaluateWindow(window, player);
            }
        }

        for (int row = 3; row < ROWS; row++)
        {
            for (int col = 0; col <= COLS - 4; col++)
            {
                int[] window = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    window[i] = board[row - i, col + i];
                }
                potentialWins += EvaluateWindow(window, player);
            }
        }

        return potentialWins;
    }

    private int EvaluateWindow(int[] window, int player)
    {
        int score = 0;
        int opponent = (player == AI) ? PLAYER : AI;

        int playerCount = window.Count(x => x == player);
        int emptyCount = window.Count(x => x == EMPTY);
        int opponentCount = window.Count(x => x == opponent);

        if (playerCount == 4) score += 100;
        else if (playerCount == 3 && emptyCount == 1) score += 5;
        else if (playerCount == 2 && emptyCount == 2) score += 2;

        if (opponentCount == 3 && emptyCount == 1) score -= 4;
        else if (opponentCount == 2 && emptyCount == 2) score -= 2;

        return score;
    }

    private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        if (CheckWin(PLAYER)) return -1000 - depth;
        if (CheckWin(AI)) return 1000 + depth;
        if (IsBoardFull()) return 0;
        if (depth == 0) return EvaluateBoard();

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            for (int col = 0; col < COLS; col++)
            {
                if (IsValidMove(col))
                {
                    MakeMove(col, AI);
                    int eval = Minimax(depth - 1, false, alpha, beta);
                    UndoMove(col);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha) break;
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int col = 0; col < COLS; col++)
            {
                if (IsValidMove(col))
                {
                    MakeMove(col, PLAYER);
                    int eval = Minimax(depth - 1, true, alpha, beta);
                    UndoMove(col);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha) break;
                }
            }
            return minEval;
        }
    }

    public int FindBestMove()
    {
        int bestMove = -1;
        int bestValue = int.MinValue;
        Random random = new Random();

        int randomFactor = random.Next(2);

        for (int col = 0; col < COLS; col++)
        {
            if (IsValidMove(col))
            {
                MakeMove(col, AI);
                int moveValue = Minimax(MAX_DEPTH, false, int.MinValue, int.MaxValue);

                if (randomFactor == 1)
                {
                    moveValue += random.Next(-10, 11);
                }

                UndoMove(col);
                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = col;
                }
                else if (moveValue == bestValue && random.Next(3) == 0)
                {
                    bestMove = col;
                }
            }
        }

        return bestMove;
    }




    public void PrintBoard()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════╗");
        Console.WriteLine("║         ████     CONNECT 4     ████        ║");
        Console.WriteLine("╠════════════════════════════════════════════╣");
        Console.WriteLine("║                GAME SCREEN                 ║");
        Console.WriteLine("║ ┌────────────────────────────────────────┐ ║");
        Console.WriteLine("║ │       1   2   3   4   5   6   7        │ ║");
        Console.WriteLine("║ │      ---------------------------       │ ║");

        for (int row = 0; row < ROWS; row++)
        {
            Console.Write("║ ");
            Console.Write("│       ");


            for (int col = 0; col < COLS; col++)
            {
                char symbol = '.';
                if (board[row, col] == PLAYER)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    symbol = 'O';
                }
                else if (board[row, col] == AI)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    symbol = 'X';
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write($"{symbol}   ");

            }
            Console.Write("     │");
            Console.Write(" ║");

            Console.WriteLine(); 
        }

        Console.ResetColor();
        Console.WriteLine("║ │      ---------------------------       │ ║");
        Console.WriteLine("║ └────────────────────────────────────────┘ ║");

        // Buttons and decorations
        Console.WriteLine("║ <<     ^^      vv        >>      oooo      ║");
        Console.WriteLine("╠════════════════════════════════════════════╣");
        Console.WriteLine("║        A        B       START    SELECT    ║");
        Console.WriteLine("╠═════════════════════╦══════════════════════╣");
        Console.WriteLine("║                     ║                      ║");
        Console.WriteLine("╚═════════════════════╩══════════════════════╝");
    }
}

class Program
{
    static void Main()
    {
        PlayIntro();

        Console.WriteLine(" __                   __                           o");
        Console.WriteLine("/  |_  _  _  _  _    /__ _ __  _    |V| _  _| _    ");
        Console.WriteLine("\\__| |(_)(_)_> (/_   \\_|(_||||(/_   | |(_)(_|(/_ o\n");

        Console.WriteLine("1. Play against AI");
        Console.WriteLine("2. Play against Human");

        int choice = 0;
        while (choice != 1 && choice != 2)
        {
            Console.Write("Enter your choice (1 or 2): ");
            int.TryParse(Console.ReadLine(), out choice);
        }

        ConnectFour game = new ConnectFour();
        if (choice == 1)
            PlayGameAgainstAI(game);
        else
            PlayGameAgainstHuman(game);
    }

    static void PlayIntro()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;

        // Top border
        Console.WriteLine("*********************************************************************************************");
        Console.WriteLine("*                                                                                           *");
        Console.WriteLine("*      ██████  ██████  ███    ██ ███    ██ ███████  ██████ ████████     ██   ██             *");
        Console.WriteLine("*     ██      ██    ██ ████   ██ ████   ██ ██      ██         ██        ██   ██             *");
        Console.WriteLine("*     ██      ██    ██ ██ ██  ██ ██ ██  ██ █████   ██         ██        ███████             *");
        Console.WriteLine("*     ██      ██    ██ ██  ██ ██ ██  ██ ██ ██      ██         ██             ██             *");
        Console.WriteLine("*      ██████  ██████  ██   ████ ██   ████ ███████  ██████    ██             ██             *");
        Console.WriteLine("*                                                                                           *");
        Console.WriteLine("*********************************************************************************************");


        // Flashing effect
        Console.ResetColor();
        System.Threading.Thread.Sleep(500);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nPress any key to start!");
        Console.ResetColor();
        Console.ReadKey(true);
        Console.Clear();
    }

    static void PlayGameAgainstAI(ConnectFour game)
    {
        Console.Clear();
        while (true)
        {
            game.PrintBoard();

            // Player's move
            PlayerMove(game, ConnectFour.PLAYER);
            game.PrintBoard(); 

            if (game.CheckWin(ConnectFour.PLAYER))
            {
                game.PrintBoard();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You win!");
                SoundPlayer player = new SoundPlayer(@"C:\Users\hamza\Downloads\Family Feud Round Win Sound Effect  Game Show Sound Effects  FREE TO USE.wav");
                player.Load();
                player.Play();
                Console.ReadLine();
                Console.ResetColor();
                break;
            }

            if (game.IsBoardFull())
            {
                game.PrintBoard();
                Console.WriteLine("It's a draw!");
                break;
            }

            Console.SetCursorPosition(2, 19);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("AI is thinking...");
            Console.ResetColor();
            int aiMove = game.FindBestMove();
            game.MakeMove(aiMove, ConnectFour.AI);
            Console.WriteLine($"AI chooses column {aiMove + 1}");
            game.PrintBoard(); 

            if (game.CheckWin(ConnectFour.AI))
            {
                game.PrintBoard();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("AI wins!");
                SoundPlayer player = new SoundPlayer(@"C:\Users\hamza\Downloads\Lose sound effects.wav");
                player.Load();
                player.Play();
                Console.ReadLine();
                Console.ResetColor();
                break;
            }

            if (game.IsBoardFull())
            {
                Console.ResetColor();
                game.PrintBoard();
                Console.WriteLine("It's a draw!");
                Console.ReadLine();
                break;
            }
        }
    }
    static void PlayGameAgainstHuman(ConnectFour game)
    {
        Console.Clear();
        while (true)
        {
            game.PrintBoard();
            // Player 1's move
            Console.SetCursorPosition(2, 19);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Player 1's Turn (O):");
            Console.SetCursorPosition(24, 19);
            Console.ResetColor();
            PlayerMove(game, ConnectFour.PLAYER);
            game.PrintBoard(); 

            if (game.CheckWin(ConnectFour.PLAYER))
            {
                game.PrintBoard();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Player 1 wins!");
                SoundPlayer player = new SoundPlayer(@"C:\Users\hamza\Downloads\Family Feud Round Win Sound Effect  Game Show Sound Effects  FREE TO USE.wav");
                player.Load();
                player.Play();
                Console.ReadLine();
                Console.ResetColor();
                break;
            }


            // Player 2's move
            Console.SetCursorPosition(2, 19);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Player 2's Turn (X):");
            Console.SetCursorPosition(24, 19);
            Console.ResetColor();
            PlayerMove(game, ConnectFour.AI);
            game.PrintBoard();  

            if (game.CheckWin(ConnectFour.AI))
            {
                game.PrintBoard();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Player 2 wins!");
                SoundPlayer player = new SoundPlayer(@"C:\Users\hamza\Downloads\Family Feud Round Win Sound Effect  Game Show Sound Effects  FREE TO USE.wav");
                player.Load();
                player.Play();
                Console.ReadLine();
                Console.ResetColor();
                break;
            }


            if (game.IsBoardFull())
            {
                Console.ResetColor();
                game.PrintBoard();
                Console.WriteLine("It's a draw!");
                Console.ReadLine();
                break;
            }
        }
    }

    static void PlayerMove(ConnectFour game, int player)
    {
        int move;
        do
        {
            Console.SetCursorPosition(24, 19);
            Console.Write("move (1-7): ");
            Console.Write("        ");
            Console.SetCursorPosition(36, 19);

        } while (!int.TryParse(Console.ReadLine(), out move) || move < 1 || move > 7 || !game.IsValidMove(move - 1));
        game.MakeMove(move - 1, player);
        
    }
}
