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
    public const int BOARD_TOP = 2;
    public ConsoleChessBoard(ChessGame game) : this(game, new()) { }
    public void Write()
    {
        WriteBoard();
        WriteCapturedPieces(game.TurnPlayer);
        WriteCapturedPieces(game.WaitingPlayer);
    }

    public void WriteCapturedPieces(PlayerInfo player)
    {
        int top = player.Player == ChessGamePlayer.Player1 ?
            BOARD_TOP + ChessBoard.TOTAL_RANKS :
            BOARD_TOP - 1;

        System.Console.SetCursorPosition(options.Left, options.Top + top);
        System.Console.ResetColor();
        SetForegroundColor(player.Player.Adversary());
        System.Console.Write(StringifyCapturedPieces(player));
    }

    private static string StringifyCapturedPieces(PlayerInfo player)
    {
        var symbols = player.CapturedPieces.Order().Select(c => c.Symbol()).ToArray();
        return new string(symbols).PadRight(20);
    }

    public void WriteBoard()
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
            SetForegroundColor(piece.Value.Player);
        }

        SetBackgroundColor(square, appearence);
        WriteSquareContent(square, piece);
    }
    private void WriteSquareContent(ChessSquare square, ChessGamePiece? piece)
    {
        System.Console.SetCursorPosition(options.Left + square.File * 2, options.Top + square.Rank + BOARD_TOP);
        var content = piece?.Piece.Symbol() ?? ' ';
        System.Console.Write(new string([content, ' ']));
    }
    private void SetForegroundColor(ChessGamePlayer player)
    {
        System.Console.ForegroundColor = player.IsPlayer1() ?
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