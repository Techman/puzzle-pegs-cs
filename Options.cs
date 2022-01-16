using CommandLine;

namespace PuzzlePegs
{
    public class Options
    {
        [Option('s', "start_pos", Required = true, HelpText = "The location of the starting hole, e.g. 13")]
        public int StartPos { get; set; }

        [Option('e', "end_pos", Required = true, HelpText = "The location of the last peg, e.g. 13")]
        public int EndPos { get; set; }
    }
}
