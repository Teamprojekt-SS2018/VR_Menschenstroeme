public class Helper {
    /* Helper */
    public static string[] SplitWhitespace(string input)
    {
        string pattern = @"\t+| +";
        return System.Text.RegularExpressions.Regex.Split(input, pattern);
    }
}
