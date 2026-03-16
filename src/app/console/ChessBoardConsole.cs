using System.Diagnostics.CodeAnalysis;
using Chess.Domain;

namespace Chess.Console;

public enum ChessBoardSquareAppearence
{
    Normal,
    Highlighted,
    Selected
}

public class ConsoleChessBoard(ChessGame game, ChessBoardConsoleOptions options)
{
    public ConsoleChessBoard(ChessGame game) : this(game, new()) { }
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

public class ChessBoardControl(
    // ChessBoardControlOptions _options
    )
{
    // public ChessBoardControl() : this(new()) { }
    public bool Read()
    {
        var control = new SequentialControl(ChessBoard.TOTAL_RANKS, ChessBoard.TOTAL_FILES);

        control.OnInput += key =>
        {
            if (key == ConsoleKey.Escape)
            {
                SelectedSquare = null;
                return;
            }

            var lastSquare = Square;
            Square = new(control.X, control.Y);

            if (Square != lastSquare)
            {
                OnChangeSquare?.Invoke(lastSquare, Square);
            }
        };

        control.OnSelect += () =>
        {
            if (!SelectedSquare.HasValue)
            {
                OnPressSelect?.Invoke((SelectedSquare = Square).Value);
                control.Read();
                return;
            }

            if (Square != SelectedSquare)
            {
                OnCompleteMove?.Invoke(new(SelectedSquare.Value, Square));
                UnselectSquare();
            }
        };

        control.Read();

        return true;
    }

    public ChessSquare Square { get; private set; }
    public ChessSquare? SelectedSquare { get; private set; }
    public bool IsOnSelectedSquare => SelectedSquare.HasValue && Square == SelectedSquare.Value;

    public ControlSquareChanged? OnChangeSquare { get; set; }
    public ControlSelectPressed? OnPressSelect { get; set; }
    public ControlMoveCompleted? OnCompleteMove { get; set; }
    public void UnselectSquare()
    {
        SelectedSquare = null;
    }
}

public delegate void ControlSquareChanged(ChessSquare previous, ChessSquare current);
public delegate void ControlSelectPressed(ChessSquare selectedSquare);
public delegate void ControlMoveCompleted(MoveSpan move);