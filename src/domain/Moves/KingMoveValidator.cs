using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class KingMoveValidator : IMoveValidator
{
    public bool MoveIsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        type = null;
        return false;
    }
}