using System.Diagnostics.CodeAnalysis;
using System.Transactions;

namespace Chess.Domain.Moves;

internal class KingMoveValidator : IMoveValidator
{
    public bool IsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        type = null;

        if (!game.TryGetOwnedPieceAndValidTarget(move, out var piece, out _) || !piece.Value.IsKing())
            return false;

        var (dy, dx) = move.OffsetAbsolute;
        bool isSimple = move.Moved && dy < 2 && dx < 2;

        // TODO castling
        bool isCastle = false;

        if (isSimple)
        {
            type = MoveType.Simple;
        }
        else if (isCastle)
        {
            // type = MoveType.LongCastling;
        }

        return isSimple || isCastle;
    }
}