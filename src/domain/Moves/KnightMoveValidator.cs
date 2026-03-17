using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class KnightMoveValidator : IMoveValidator
{
    public bool IsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        bool valid = game.TryGetOwnedPieceAndValidTarget(move, out var piece, out _)
            && piece.Value.IsKnight()
            && move.IsLShaped();

        type = valid ? MoveType.Simple : null;

        return valid;
    }
}