namespace Chess.Console;

public class ConsoleColorSelect(ConsoleColorSelectOptions _options)
{
    private const int COLOR_TILE_SIZE = 2;
    private int _colorIndex = 0;
    private readonly ConsoleColor[] _colors = Enum.GetValues<ConsoleColor>();
    public ConsoleColorSelect() : this(new()) { }
    public Action<ConsoleColor>? OnSelect { get; set; }
    public Action<ConsoleColor>? OnChange { get; set; }
    public ConsoleColor Value => _colors[_colorIndex];

    public void Select()
    {
        _colorIndex = 0;
        DisplayPalette();
        GetControl().Read();
    }

    private SequentialControl GetControl()
    {
        var control = SequentialControl.Horizontal(_colors.Length);

        control.OnInput += _ =>
        {
            int lastIndex = _colorIndex;
            _colorIndex = control.X;
            OnChangeIndex(lastIndex);
        };

        control.OnSelect += () => OnSelect?.Invoke(Value);

        return control;
    }

    private void OnChangeIndex(int lastIndex)
    {
        if (lastIndex == _colorIndex)
            return;

        DisplayCursor(lastIndex, false);
        DisplayCursor(_colorIndex);
        OnChange?.Invoke(Value);
    }
    private void DisplayCursor(int index, bool display = true)
    {
        System.Console.SetCursorPosition(_options.Left + index * COLOR_TILE_SIZE, _options.Top + 1);
        System.Console.ResetColor();
        System.Console.Write(display ? '^' : ' ');
    }
    private void DisplayPalette()
    {
        int index = 0;
        foreach (var color in _colors)
        {
            System.Console.BackgroundColor = color;
            System.Console.SetCursorPosition(_options.Left + COLOR_TILE_SIZE * index++, _options.Top);
            System.Console.Write(string.Empty.PadRight(COLOR_TILE_SIZE));
        }
        System.Console.ResetColor();
        DisplayCursor(_colorIndex);
    }
}

public class ConsoleColorSelectOptions
{
    public int Top { get; init; }
    public int Left { get; init; }
}