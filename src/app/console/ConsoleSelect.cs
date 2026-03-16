using System.Drawing;

namespace Chess.Console;

public class ConsoleSelect<TItem, TValue>
    where TItem : IConsoleSelectItem<TValue> where TValue : notnull
{
    private readonly TItem[] _items;
    private readonly ConsoleSelectOptions _options;
    private int _itemIndex = 0;
    public ConsoleSelect(TItem[] items, ConsoleSelectOptions options)
    {
        _items = items;
        _options = options;
        Value = _items[0].Value; // TODO initial value argument
        _itemIndex = _items.IndexOf(SelectedItem);
    }
    public ConsoleSelect(TItem[] items) : this(items, new()) { }
    public void Select()
    {
        WriteHorizontally();

        ConsoleKey input;

        while((input = Utils.ReadKey()) != _options.SelectKey)
        {
            
        }
    }

    public TValue Value { get; private set; }
    public TItem SelectedItem => FindItemByValue(Value);
    private TItem FindItemByValue(TValue value) => _items.Single(i => i.Value.Equals(value));

    private void WriteHorizontally()
    {
        foreach (var item in _items)
        {
            bool isSelected = item.Value.Equals(Value);
            var label = isSelected ? $"[{item}]" : $" {item} ";

            System.Console.ForegroundColor = isSelected ? _options.SelectedColor : _options.Color;
            System.Console.Write(label);
            System.Console.CursorLeft += _options.Gap;
        }

        System.Console.ResetColor();
    }
}

public record ConsoleSelectOptions
{
    public int Gap { get; init; } = 3;
    public int Left { get; init; }
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
