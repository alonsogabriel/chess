using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

public interface IMoveValidator
{
    bool MoveIsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type);
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

