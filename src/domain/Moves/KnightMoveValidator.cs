using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class KnightMoveValidator : IMoveValidator
{
    public bool MoveIsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        bool valid = game.TryGetMovedPiece(move, out var piece)
            && piece.Value.IsKnight()
            && move.IsLShaped();

        type = valid ? MoveType.Simple : null;

        return valid;
    }
}