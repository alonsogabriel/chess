namespace Chess.Domain;

public enum ChessPiece
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum ChessGamePlayer
{
    Player1,
    Player2
}

public readonly record struct ChessGamePiece
{
    public ChessGamePiece() : this(ChessPiece.Pawn, ChessGamePlayer.Player1) { }
    public ChessGamePiece(ChessPiece piece, ChessGamePlayer player)
    {
        if (!Enum.IsDefined(piece))
            throw new ArgumentException("Invalid chess piece.", nameof(piece));

        if (!Enum.IsDefined(player))
            throw new ArgumentException("Invalid chess player.", nameof(player));

        Piece = piece;
        Player = player;
    }
    public ChessPiece Piece { get; private init; }
    public ChessGamePlayer Player { get; private init; }

    public override string ToString()
    {
        return $"{Piece.Letter()}{(int)Player + 1}";
    }
}