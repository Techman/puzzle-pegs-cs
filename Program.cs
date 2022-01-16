using System;
using System.Collections.Generic;
using CommandLine;

namespace PuzzlePegs
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parse arguments
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                    // Good parse
                    .WithParsed<Options>(RunPuzzle)
                    // Bad parse
                    .WithNotParsed<Options>(HandleErrors);
            }
            catch (ArgumentNullException e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error parsing arguments");
                Console.Error.WriteLine(e.Message);
            }
        }

        static void RunPuzzle(Options opts)
        {
            // Due to the parsing library in use, I could not manually specify acceptable values,
            // only a range. For EndPos, treat 0 as -1 since the range is [-1, 15]
            var startPos = opts.StartPos;
            var endPos = opts.EndPos;

            // Check that values are within range
            if (!Utils.CheckBetween(1, 15, startPos))
            {
                Console.Error.WriteLine("Starting hole must range [1, 15]");
                return;
            }
            if (!Utils.CheckBetween(-1, 15, endPos))
            {
                Console.Error.WriteLine("Ending peg must range from [-1, 1 - 15] (-1 indicates no preference)");
                return;
            }

            // Check if endPos is 0
            if (endPos == 0)
            {
                Console.WriteLine("NOTE: Treating endPos value of 0 as -1 (no preference)");
                endPos = -1;
            }

            // Build puzzle
            var puzzle = new PuzzlePegs(startPos, endPos);

            // Solve
            puzzle.Solve();
        }

        static void HandleErrors(IEnumerable<Error> errs)
        {
            Console.Error.WriteLine("Error(s) parsing input:");
            foreach (var e in errs)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }
    }
}
