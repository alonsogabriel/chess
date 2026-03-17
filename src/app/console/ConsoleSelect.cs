namespace Chess.Console;

public class ConsoleSelect<TItem, TValue>
    where TItem : IConsoleSelectItem<TValue> where TValue : notnull
{
    private readonly TItem[] items;
    private readonly ConsoleSelectOptions options;
    public ConsoleSelect(TItem[] items, ConsoleSelectOptions options)
    {
        this.items = items;
        this.options = options;
        Value = this.items[0].Value; // TODO initial value argument
    }
    public ConsoleSelect(TItem[] items) : this(items, new()) { }
    public void Select()
    {
        WriteHorizontally();
        GetControl().Read();
    }

    public TValue Value { get; private set; }
    public TItem SelectedItem => FindItemByValue(Value);

    public Action<TValue>? OnSelect { get; set; }
    public Action<ConsoleKey>? OnInput { get; set; }
    private TItem FindItemByValue(TValue value) => items.Single(i => i.Value.Equals(value));
    private SequentialControl GetControl()
    {
        var control = SequentialControl.Horizontal(items.Length, keys: new() { Escape = [ConsoleKey.Enter] });

        control.OnInput += (ctrl, key) =>
        {
            HandleValue(ctrl);
            OnInput?.Invoke(key);
        };

        control.OnSelect += _ => OnSelect?.Invoke(Value);

        return control;
    }

    private void HandleValue(SequentialControl control)
    {
        var newValue = items[control.X].Value;

        if (newValue.Equals(Value))
            return;

        Value = newValue;
        WriteHorizontally();
    }
    private void WriteHorizontally()
    {
        System.Console.ResetColor();
        System.Console.SetCursorPosition(options.Left, options.Top);

        foreach (var item in items)
        {
            bool isSelected = item.Value.Equals(Value);
            var label = isSelected ? $"[{item}]" : $" {item} ";

            System.Console.ForegroundColor = isSelected ? options.SelectedColor : options.Color;
            System.Console.Write(label);
            System.Console.CursorLeft += options.Gap;
        }

        System.Console.ResetColor();
    }
}

public record ConsoleSelectOptions
{
    public int Gap { get; init; } = 3;
    public int Left { get; init; }
    public int Top { get; init; }
    public ConsoleColor Color { get; init; }
    public ConsoleColor SelectedColor { get; init; } = ConsoleColor.Cyan;
    public ConsoleKey SelectKey { get; init; } = ConsoleKey.Spacebar;
}

public class ConsoleSelect<TItem>(TItem[] items, ConsoleSelectOptions options)
    : ConsoleSelect<TItem, int>(items, options) where TItem : IConsoleSelectItem<int>
{
    public ConsoleSelect(TItem[] items) : this(items, new()) { }
}

public interface IConsoleSelectItem<T> where T : notnull
{
    T Value { get; }
}

public readonly record struct SelectItem(string Label, int Value) : IConsoleSelectItem<int>
{
    public override string ToString()
    {
        return Label;
    }
}
