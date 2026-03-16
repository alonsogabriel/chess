namespace Chess.Console;

public class SequentialControl(int yCount, int xCount, int y, int x, SequentialControlKeys keys)
{
    public SequentialControl(int yCount, int xCount) : this(yCount, xCount, 0, 0, new()) { }

    public static SequentialControl Horizontal(int count, int x = 0, SequentialControlKeys? keys = null)
    {
        return new(0, count, 0, x, keys ?? new());
    }

    public static SequentialControl Vertical(int count, int y = 0, SequentialControlKeys? keys = null)
    {
        return new(count, 0, y, 0, keys ?? new());
    }

    public int Y { get; private set; } = Utils.InRange(y, 0, yCount - 1);
    public int X { get; private set; } = Utils.InRange(x, 0, xCount - 1);

    public void Read()
    {
        while (Input()) ;

        OnSelect?.Invoke();
    }

    public Action<ConsoleKey>? OnInput { get; set; }
    public Action? OnSelect { get; set; }

    private bool Input()
    {
        ConsoleKey key = Utils.ReadKey();
        HandleInput(key);
        OnInput?.Invoke(key);

        return key != keys.Select;
    }

    private void HandleInput(ConsoleKey key)
    {
        if (key == keys.Up)
        {
            MoveUp();
            return;
        }

        if (key == keys.Right)
        {
            MoveRight();
            return;
        }

        if (key == keys.Down)
        {
            MoveDown();
            return;
        }

        if (key == keys.Left)
        {
            MoveLeft();
        }
    }

    private void MoveUp() => Y = HandleIndex(--Y, yCount);
    private void MoveRight() => X = HandleIndex(++X, xCount);
    private void MoveDown() => Y = HandleIndex(++Y, yCount);
    private void MoveLeft() => X = HandleIndex(--X, xCount);

    private static int HandleIndex(int index, int count)
    {
        return count != 0 ? (count + index) % count : 0;
    }
}

public record class SequentialControlKeys
{
    public ConsoleKey Up { get; init; } = ConsoleKey.UpArrow;
    public ConsoleKey Right { get; init; } = ConsoleKey.RightArrow;
    public ConsoleKey Down { get; init; } = ConsoleKey.DownArrow;
    public ConsoleKey Left { get; init; } = ConsoleKey.LeftArrow;
    public ConsoleKey Select { get; init; } = ConsoleKey.Spacebar;
}