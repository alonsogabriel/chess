using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class BishopMoveValidator : IMoveValidator
{
    public bool IsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        bool valid = game.TryGetOwnedPieceAndValidTarget(move, out var piece, out _)
            && piece.Value.IsBishop()
            && move.IsDiagonal()
            && !game.PathIsBlocked(move);

        type = valid ? MoveType.Simple : null;

        return valid;
    }
}
