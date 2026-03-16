namespace Chess.Console;

public static class Utils
{
    public static ConsoleKey ReadKey() => System.Console.ReadKey(true).Key;
    public static int InRange(this int value, int min, int max)
    {
        max = Math.Max(min, max);
        min = Math.Min(min, max);

        return Math.Min(Math.Max(value, min), max);
    }
}
