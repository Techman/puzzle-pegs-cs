namespace PuzzlePegs
{
    public class Utils
    {
        // TODO: Investigate making this generic
        // Does not seem to be as simple as
        // CheckBetween<T>(T lower, T upper, T value)
        static public bool CheckBetween(int lower, int upper, int value)
        {
            if ((value >= lower) && (value <= upper))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
