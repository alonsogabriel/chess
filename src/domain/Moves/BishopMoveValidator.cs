using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class BishopMoveValidator : IMoveValidator
{
    public bool MoveIsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        bool valid = game.TryGetMovedPiece(move, out var piece)
            && piece.Value.IsBishop()
            && move.IsDiagonal()
            && !game.PathIsBlocked(move);

        type = valid ? MoveType.Simple : null;

        return valid;
    }
}
