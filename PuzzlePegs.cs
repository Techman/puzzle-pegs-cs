using System;
using System.Collections.Generic;

// https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

namespace PuzzlePegs
{
    public class PuzzlePegs
    {
        // Universal representation of a peg
        private static char s_peg = 'P';

        // Universal representation of a hole
        private static char s_hole = 'H';

        // Table of all possible moves
        private List<int[]> s_moves;

        // History of boards representing the jumps
        private List<char[]> boards;

        // History of jumps
        private List<string> jumps;

        // Starting hole location

        private int startPos;

        // Ending hole location
        private int endPos;

        public PuzzlePegs(int startPos, int endPos)
        {
            // Start start and end locations
            this.startPos = startPos;
            this.endPos = endPos;

            // Initalize lists
            boards = new List<char[]>();
            jumps = new List<string>();

            // Set up table of moves
            // I am using a List<T> because apparently C# does not like 2D normal arrays...cannot
            // iterate through to pull each array from within the array like I want. This also
            // means that I cannot make the array static, so extra memory will be used.
            s_moves = new List<int[]>(36);
            s_moves.Add(new int[] { 1, 2, 4 });
            s_moves.Add(new int[] { 1, 3, 6 });
            s_moves.Add(new int[] { 2, 4, 7 });
            s_moves.Add(new int[] { 2, 5, 9 });
            s_moves.Add(new int[] { 3, 5, 8 });
            s_moves.Add(new int[] { 3, 6, 10 });
            s_moves.Add(new int[] { 4, 2, 1 });
            s_moves.Add(new int[] { 4, 5, 6 });
            s_moves.Add(new int[] { 4, 7, 11 });
            s_moves.Add(new int[] { 4, 8, 13 });
            s_moves.Add(new int[] { 5, 8, 12 });
            s_moves.Add(new int[] { 5, 9, 14 });
            s_moves.Add(new int[] { 6, 3, 1 });
            s_moves.Add(new int[] { 6, 5, 4 });
            s_moves.Add(new int[] { 6, 9, 13 });
            s_moves.Add(new int[] { 6, 10, 15 });
            s_moves.Add(new int[] { 7, 4, 2 });
            s_moves.Add(new int[] { 7, 8, 9 });
            s_moves.Add(new int[] { 8, 5, 3 });
            s_moves.Add(new int[] { 8, 9, 10 });
            s_moves.Add(new int[] { 9, 5, 2 });
            s_moves.Add(new int[] { 9, 8, 7 });
            s_moves.Add(new int[] { 10, 6, 3 });
            s_moves.Add(new int[] { 10, 9, 8 });
            s_moves.Add(new int[] { 11, 7, 4 });
            s_moves.Add(new int[] { 11, 12, 13 });
            s_moves.Add(new int[] { 12, 8, 5 });
            s_moves.Add(new int[] { 12, 13, 14 });
            s_moves.Add(new int[] { 13, 12, 11 });
            s_moves.Add(new int[] { 13, 8, 4 });
            s_moves.Add(new int[] { 13, 9, 6 });
            s_moves.Add(new int[] { 13, 14, 15 });
            s_moves.Add(new int[] { 14, 13, 12 });
            s_moves.Add(new int[] { 14, 9, 5 });
            s_moves.Add(new int[] { 15, 10, 6 });
            s_moves.Add(new int[] { 15, 14, 13 });
        }

        private static int Count(char[] array, char value)
        {
            var count = 0;
            foreach (var c in array)
            {
                if (c == value)
                {
                    count++;
                }
            }
            return count;
        }

        public bool Solve()
        {
            // Build the board. Reserve 16 spaces.
            var board = new char[16];
            board[0] = ' '; // This space is not used
            for (var i = 1; i < 16; ++i)
            {
                if (startPos == i)
                {
                    board[i] = s_hole;
                }
                else
                {
                    board[i] = s_peg;
                }
            }

            // Store the original board to show before moves are printed
            var original = new char[16];
            Array.Copy(board, original, board.Length);

            // Now, solve the puzzle!
            if (SolveInternal(board))
            {
                Console.WriteLine("Initial board");
                PrintBoard(original);

                // Print the moves and board to the output. The moves (jumps) are in reverse order
                // due to the recursion. The board states are not.
                jumps.Reverse();
                for (var i = 0; i < boards.Count; ++i)
                {
                    Console.WriteLine(jumps[i]);
                    PrintBoard(boards[i]);
                }
                return true;
            }
            else
            {
                Console.WriteLine("No solution could be found for this combination");
                return false;
            }
        }

        private bool SolveInternal(char[] board)
        {
            // For every move in the table of possible moves...
            foreach (var move in s_moves)
            {
                // See if we can match a PPH pattern. If we can, try following this route by
                // calling ourselves again with this modified board
                if ((board[move[0]] == s_peg) && (board[move[1]] == s_peg) && (board[move[2]] == s_hole))
                {
                    // Apply the move
                    board[move[0]] = s_hole;
                    board[move[1]] = s_hole;
                    board[move[2]] = s_peg;

                    // Record the board in history of boards
                    var clone = new char[board.Length];
                    Array.Copy(board, clone, board.Length);
                    boards.Add(clone);

                    // Call ourselves recursively. IF we return true then the conclusion was good.
                    // If it was false, we hit a dead end and we shouldn't print the board
                    if (SolveInternal(board))
                    {
                        // Record the jump
                        var jump = "Moved " + move[0] + " to " + move[2] + ", jumping over " + move[1];
                        jumps.Add(jump);
                        return true;
                    }

                    // If we end up here, undo the move and try the next one
                    boards.RemoveAt(boards.Count - 1);
                    board[move[0]] = s_peg;
                    board[move[1]] = s_peg;
                    board[move[2]] = s_hole;
                }
            }

            // If no pattern is matched, see if there is only one peg gleft and see if it is in the
            // right spot.
            // Situation 1: Count of peg is 1 and the value of the ending position was not specified
            var pegCount = Count(board, s_peg);
            if ((pegCount == 1) && (endPos == -1))
            {
                return true;
            }
            // Situation 2: Count of peg is 1 and the value of the ending position was peg
            else if ((pegCount == 1) && (board[endPos] == s_peg))
            {
                return true;
            }
            // Situation 3: count of peg was not 1 or the value at the ending position was not peg
            else
            {
                return false;
            }
        }

        private static void PrintBoard(char[] board)
        {
            Console.WriteLine("    " + board[1]);
            Console.WriteLine("   " + board[2] + " " + board[3]);
            Console.WriteLine("  " + board[4] + " " + board[5] + " " + board[6]);
            Console.WriteLine(" " + board[7] + " " + board[8] + " " + board[9] + " " + board[10]);
            Console.WriteLine(board[11] + " " + board[12] + " " + board[13] + " " + board[14] + " " + board[15]);
        }

        public static void Help()
        {
            Console.WriteLine("Usage: PuzzlePegs [start_pos] [end_pos]");
            Console.WriteLine("start_pos: The location of the starting hole, e.g. 13");
            Console.WriteLine("end_pos: The location of the last peg, e.g. 13");
        }
    }
}
