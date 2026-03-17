using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class RookMoveValidator : IMoveValidator
{
    public bool IsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        bool valid = game.TryGetOwnedPieceAndValidTarget(move, out var piece, out _)
            && piece.Value.IsRook()
            && move.IsOrthogonal()
            && !game.PathIsBlocked(move);
        

        type = valid ? MoveType.Simple : null;

        return valid;
    }
}