using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class QueenMoveValidator : IMoveValidator
{
    public bool IsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        bool valid = game.TryGetOwnedPieceAndValidTarget(move, out var piece, out _)
            && piece.Value.IsQueen()
            && move.IsOrthogonalOrDiagonal()
            && !game.PathIsBlocked(move);

        type = valid ? MoveType.Simple : null;

        return valid;
    }
}