namespace Chess.Domain;

public class PlayerInfo
{
    private readonly List<ChessPiece> _capturedPieces = [];
    public ChessGamePlayer Player { get; set; }
    public string Name { get; set; }
    public bool KingHasMoved { get; set; }
    public bool QueenSideRookHasMoved { get; set; }
    public bool KingSideRookHasMoved { get; set; }
    public ChessSquare? AvailableEnPassantSquare { get; set; }
    public bool IsInCheck { get; set; }
    public IReadOnlyList<ChessPiece> CapturedPieces => _capturedPieces;

    internal void AddCapturedPiece(ChessPiece piece) => _capturedPieces.Add(piece);
}