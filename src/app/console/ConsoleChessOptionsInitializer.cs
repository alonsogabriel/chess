using System.Text.Json;
using Chess.Domain;

namespace Chess.Console;

public class ConsoleChessOptionsInitializer
{
    private readonly string optionsPath = Path.Combine(AppContext.BaseDirectory, "chess.config.json");
    private readonly ConsoleSelect<SelectItem> colorComponentSelect = new(Utils.ChessBoardComponentColorItems());
    private readonly ConsoleColorSelect colorSelect = new(new() { Top = 2 });
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };
    public async Task<ConsoleChessBoardOptions> Initialize()
    {
        var options = await LoadOptionsAsync() with { Top = 4 };
        var component = ChessBoardColorComponent.Square1;
        var board = new ConsoleChessBoard(new(), options);

        colorComponentSelect.OnSelect += value =>
        {
            component = (ChessBoardColorComponent)value;
            colorSelect.Select();
        };

        colorSelect.OnChange += (color) =>
        {
            SetComponentColor(options, component, color);
            board.Write();
            board.WriteSquare(ChessSquare.From("d5"), ChessBoardSquareAppearence.Highlighted);
            board.WriteSquare(ChessSquare.From("e4"), ChessBoardSquareAppearence.Selected);
        };

        colorSelect.OnSelect += (value) =>
        {
            colorComponentSelect.Select();
        };

        colorComponentSelect.Select();

        options = options with { Top = 0 };
        await SaveOptionsAsync(options);

        return options;
    }

    private async Task<ConsoleChessBoardOptions> LoadOptionsAsync()
    {
        if (!File.Exists(optionsPath))
            return new();

        var json = await File.ReadAllTextAsync(optionsPath);

        return JsonSerializer.Deserialize<ConsoleChessBoardOptions>(json, jsonOptions)
            ?? throw new InvalidOperationException("Invalid config file. Delete the file and try again.");
    }

    private async Task SaveOptionsAsync(ConsoleChessBoardOptions options)
    {
        var json = JsonSerializer.Serialize(options);
        await File.WriteAllTextAsync(optionsPath, json);
    }

    private static void SetComponentColor(ConsoleChessBoardOptions options, ChessBoardColorComponent component, ConsoleColor color)
    {
        if (component == ChessBoardColorComponent.Square1)
        {
            options.BoardColor1 = color;
            return;
        }

        if (component == ChessBoardColorComponent.Square2)
        {
            options.BoardColor2 = color;
            return;
        }

        if (component == ChessBoardColorComponent.Player1)
        {
            options.Player1Color = color;
            return;
        }

        if (component == ChessBoardColorComponent.Player2)
        {
            options.Player2Color = color;
            return;
        }

        if (component == ChessBoardColorComponent.SelectedSquare)
        {
            options.SelectColor = color;
            return;
        }

        if (component == ChessBoardColorComponent.HighlightedSquare)
        {
            options.HighlightColor = color;
        }
    }
}
