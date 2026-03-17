using Chess.Domain;

namespace Chess.Console;

public enum ChessBoardSquareAppearence
{
    Normal,
    Highlighted,
    Selected
}

public class ConsoleChessBoard(ChessGame game, ConsoleChessBoardOptions options)
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
            SetForegroundColor(piece.Value);
        }

        SetBackgroundColor(square, appearence);
        WriteSquareContent(square, piece);
    }
    private void WriteSquareContent(ChessSquare square, ChessGamePiece? piece)
    {
        System.Console.SetCursorPosition(options.Left + square.File * 2, options.Top + square.Rank);
        var content = piece?.Piece.Symbol() ?? ' ';
        System.Console.Write(new string([content, ' ']));
    }
    private void SetForegroundColor(ChessGamePiece piece)
    {
        System.Console.ForegroundColor = piece.Player.IsPlayer1() ?
            options.Player1Color :
            options.Player2Color;
    }
    private void SetBackgroundColor(ChessSquare square, ChessBoardSquareAppearence appearence)
    {
        System.Console.BackgroundColor = GetBackgroundColor(square, appearence);
    }
    private ConsoleColor GetBackgroundColor(ChessSquare square, ChessBoardSquareAppearence appearence)
    {
        return appearence switch
        {
            ChessBoardSquareAppearence.Highlighted => options.HighlightColor,
            ChessBoardSquareAppearence.Selected => options.SelectColor,
            _ => square.Rank % 2 == square.File % 2 ?
                   options.BoardColor1 :
                   options.BoardColor2
        };
    }
}

public record ConsoleChessBoardOptions
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