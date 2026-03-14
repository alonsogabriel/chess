namespace Chess.Domain;

public class PlayerInfo
{
    public ChessGamePlayer Player { get; set; }
    public string Name { get; set; }
    public bool KingHasMoved { get; set; }
    public bool QueenSideRookHasMoved { get; set; }
    public bool KingSideRookHasMoved { get; set; }
    public ChessSquare? AvailableEnPassantSquare { get; set; }
    public bool IsInCheck { get; set; }
}