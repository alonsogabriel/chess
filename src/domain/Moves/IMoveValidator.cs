using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

public interface IMoveValidator
{
    bool IsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type);
}

public enum MoveType
{
    Simple,
    DoubleStep,
    EnPassant,
    ShortCastling,
    LongCastling,
    Promotion
}

public static class MoveValidator
{
    private static readonly Dictionary<ChessPiece, IMoveValidator> _validators = new()
    {
        [ChessPiece.Pawn] = new PawnMoveValidator(),
        [ChessPiece.Knight] = new KnightMoveValidator(),
        [ChessPiece.Bishop] = new BishopMoveValidator(),
        [ChessPiece.Rook] = new RookMoveValidator(),
        [ChessPiece.Queen] = new QueenMoveValidator(),
        [ChessPiece.King] = new KingMoveValidator()
    };
    public static IMoveValidator From(ChessPiece piece) => _validators[piece];
    public static IMoveValidator From(ChessGamePiece piece) => _validators[piece.Piece];
}

