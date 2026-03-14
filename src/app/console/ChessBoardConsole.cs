using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Chess.Domain;

namespace Chess.Console;

public enum ChessBoardSquareAppearence
{
    Normal,
    Highlighted,
    Selected
}

public class ChessBoardConsole(ChessGame game, ChessBoardConsoleOptions options)
{
    public ChessBoardConsole(ChessGame game) : this(game, new()) { }
    public void Write()
    {
        for (int rank = 0; rank < ChessBoard.TOTAL_RANKS; rank++)
        {
            for (int file = 0; file < ChessBoard.TOTAL_FILES; file++)
            {
                WriteSquare(new(file, rank));
            }
        }
    }

    public void WriteSquare(ChessSquare square, ChessBoardSquareAppearence appearence = default)
    {
        if (game.TryGetPiece(square, out var piece))
        {
            System.Console.ForegroundColor =
                piece.Value.Player.IsPlayer1() ?
                    options.Player1Color :
                    options.Player2Color;
        }
        if (appearence == ChessBoardSquareAppearence.Highlighted)
        {
            System.Console.BackgroundColor = options.HighlightColor;
        }
        else if (appearence == ChessBoardSquareAppearence.Selected)
        {
            System.Console.BackgroundColor = options.SelectColor;
        }
        else
        {
            System.Console.BackgroundColor =
                square.Rank % 2 == square.File % 2 ?
                    options.BoardColor1 :
                    options.BoardColor2;
        }

        System.Console.SetCursorPosition(options.Left + square.File * 2, options.Top + square.Rank);
        var content = piece?.Piece.Symbol() ?? ' ';
        System.Console.Write(new string([content, ' ']));
    }
}

public class ChessBoardConsoleOptions
{
    public int Left { get; set; } = 0;
    public int Top { get; set; } = 0;
    public ConsoleColor Player1Color { get; set; } = ConsoleColor.DarkRed;
    public ConsoleColor Player2Color { get; set; } = ConsoleColor.Black;
    public ConsoleColor BoardColor1 { get; set; } = ConsoleColor.Gray;
    public ConsoleColor BoardColor2 { get; set; } = ConsoleColor.DarkGray;
    public ConsoleColor HighlightColor { get; set; } = ConsoleColor.Yellow;
    public ConsoleColor SelectColor { get; set; } = ConsoleColor.Green;
}

public class ChessBoardControl(ChessBoardControlOptions _options)
{
    public ChessBoardControl() : this(new()) { }
    private int y = 0;
    private int x = 0;
    public bool Read([NotNullWhen(true)] out ChessBoardControlKey? key)
    {
        var input = System.Console.ReadKey(true).Key;
        var lastSquare = Square;

        if (input == _options.Keys[(key = ChessBoardControlKey.Up).Value])
        {
            y = HandleAxisValue(--y, ChessBoard.TOTAL_RANKS);
        }
        else if (input == _options.Keys[(key = ChessBoardControlKey.Down).Value])
        {
            y = HandleAxisValue(++y, ChessBoard.TOTAL_RANKS);
        }
        else if (input == _options.Keys[(key = ChessBoardControlKey.Left).Value])
        {
            x = HandleAxisValue(--x, ChessBoard.TOTAL_FILES);
        }
        else if (input == _options.Keys[(key = ChessBoardControlKey.Right).Value])
        {
            x = HandleAxisValue(++x, ChessBoard.TOTAL_FILES);
        }
        else if (input == _options.Keys[(key = ChessBoardControlKey.Select).Value])
        {
            var sq = Square;
            if (SelectedSquare.HasValue && sq != SelectedSquare)
            {
                OnCompleteMove?.Invoke(new(SelectedSquare.Value, sq));
                UnselectSquare();
            }
            else
            {
                OnPressSelect?.Invoke((SelectedSquare = sq).Value);
            }
        }
        else if (input == _options.Keys[(key = ChessBoardControlKey.Esc).Value])
        {
            SelectedSquare = null;
        }
        else
        {
            key = null;
        }

        var currentSquare = Square;

        if (currentSquare != lastSquare)
        {
            OnChangeSquare?.Invoke(lastSquare, currentSquare);
        }

        return key is not null;
    }

    public ChessSquare Square => new(x, y);
    public ChessSquare? SelectedSquare { get; private set; }
    public bool IsOnSelectedSquare => SelectedSquare.HasValue && Square == SelectedSquare.Value;

    public ControlSquareChanged? OnChangeSquare { get; set; }
    public ControlSelectPressed? OnPressSelect { get; set; }
    public ControlMoveCompleted? OnCompleteMove { get; set; }
    public void UnselectSquare()
    {
        SelectedSquare = null;
    }

    private static int HandleAxisValue(int value, int limit)
    {
        return (value + limit) % limit;
    }
}

public delegate void ControlSquareChanged(ChessSquare previous, ChessSquare current);
public delegate void ControlSelectPressed(ChessSquare selectedSquare);
public delegate void ControlMoveCompleted(MoveSpan move);

public enum ChessBoardControlKey
{
    Up,
    Down,
    Left,
    Right,
    Select,
    Esc,
    // Chat,
}

public class ChessBoardControlOptions
{
    private readonly Dictionary<ChessBoardControlKey, ConsoleKey> _keys = [];

    public ChessBoardControlOptions()
    {
        this
            .UseUpKey(ConsoleKey.W)
            .UseDownKey(ConsoleKey.S)
            .UseLeftKey(ConsoleKey.A)
            .UseRightKey(ConsoleKey.D)
            .UseSelectKey(ConsoleKey.Spacebar)
            .UseEscKey(ConsoleKey.Escape);
    }

    public IReadOnlyDictionary<ChessBoardControlKey, ConsoleKey> Keys => _keys;

    public ChessBoardControlOptions UseUpKey(ConsoleKey key)
    {
        _keys[ChessBoardControlKey.Up] = key;
        return this;
    }

    public ChessBoardControlOptions UseDownKey(ConsoleKey key)
    {
        _keys[ChessBoardControlKey.Down] = key;
        return this;
    }

    public ChessBoardControlOptions UseLeftKey(ConsoleKey key)
    {
        _keys[ChessBoardControlKey.Left] = key;
        return this;
    }

    public ChessBoardControlOptions UseRightKey(ConsoleKey key)
    {
        _keys[ChessBoardControlKey.Right] = key;
        return this;
    }

    public ChessBoardControlOptions UseSelectKey(ConsoleKey key)
    {
        _keys[ChessBoardControlKey.Select] = key;
        return this;
    }

    public ChessBoardControlOptions UseEscKey(ConsoleKey key)
    {
        _keys[ChessBoardControlKey.Esc] = key;
        return this;
    }
}