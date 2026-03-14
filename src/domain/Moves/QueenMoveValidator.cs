using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class QueenMoveValidator : IMoveValidator
{
    public bool MoveIsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        bool valid = game.TryGetMovedPiece(move, out var piece)
            && piece.Value.IsQueen()
            && move.IsOrthogonalOrDiagonal()
            && !game.PathIsBlocked(move);

        type = valid ? MoveType.Simple : null;

        return valid;
    }
}