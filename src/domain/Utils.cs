namespace Chess.Domain;

public static class Utils
{
    public static bool IsPawn(this ChessPiece piece) => piece == ChessPiece.Pawn;
    public static bool IsKnight(this ChessPiece piece) => piece == ChessPiece.Knight;
    public static bool IsBishop(this ChessPiece piece) => piece == ChessPiece.Bishop;
    public static bool IsRook(this ChessPiece piece) => piece == ChessPiece.Rook;
    public static bool IsQueen(this ChessPiece piece) => piece == ChessPiece.Queen;
    public static bool IsKing(this ChessPiece piece) => piece == ChessPiece.King;
    public static char Letter(this ChessPiece piece)
    {
        if (piece.IsKnight())
            return 'N';

        return Enum.GetName(piece)?.First() ?? '?';
    }
}