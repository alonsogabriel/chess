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

    public static SelectItem[] ChessBoardComponentColorItems()
    {
        return [.. Enum.GetValues<ChessBoardColorComponent>().Select(c => new SelectItem(c.Label(), (int)c))];
    }

    private static string Label(this ChessBoardColorComponent component)
    {
        return component switch
        {
            ChessBoardColorComponent.Square1 => "Square 1",
            ChessBoardColorComponent.Square2 => "Square 2",
            ChessBoardColorComponent.Player1 => "Player 1",
            ChessBoardColorComponent.Player2 => "Player 2",
            ChessBoardColorComponent.SelectedSquare => "Selected Square",
            ChessBoardColorComponent.HighlightedSquare => "Highlighted Square",
            _ => string.Empty
        };
    }
}
