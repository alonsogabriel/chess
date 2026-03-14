using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain.Moves;

internal class PawnMoveValidator : IMoveValidator
{
    public bool MoveIsValid(MoveSpan move, ChessGame game, [NotNullWhen(true)] out MoveType? type)
    {
        type = null;

        if (!game.TryGetMovedPiece(move, out var piece) || !piece.Value.IsPawn())
            return false;

        return IsSimple(move, game, out type)
            || IsDoubleStep(move, game, out type)
            || IsCapture(move, game, out type)
            || IsEnPassant(move, game, out type);
    }

    private static bool IsSimple(MoveSpan move, ChessGame game, out MoveType? type)
    {
        bool valid = move.IsVertical()
            && move.OffsetRank == game.TurnPlayer.Player.RankDirection()
            && !game.TryGetTargetPiece(move, out _);

        type = valid ? MoveType.Simple : null;

        return valid;
    }

    private static bool IsDoubleStep(MoveSpan move, ChessGame game, out MoveType? type)
    {
        int direction = game.TurnPlayer.Player.RankDirection();

        bool valid = move.From.Rank == game.TurnPlayer.Player.PawnRank()
            && move.IsVertical()
            && move.OffsetRank == direction * 2
            && !game.TryGetPiece(move.From.AddRanks(direction), out _);

        type = valid ? MoveType.DoubleStep : null;

        return valid;
    }

    private static bool IsCapture(MoveSpan move, ChessGame game, out MoveType? type)
    {
        bool valid = move.IsDiagonal()
            && move.OffsetRank == game.TurnPlayer.Player.RankDirection()
            && game.TryGetTargetPiece(move, out var _);

        type = valid ? MoveType.Simple : null;

        return valid;
    }

    private static bool IsEnPassant(MoveSpan move, ChessGame game, out MoveType? type)
    {
        var enPassantSquare = game.TurnPlayer.AvailableEnPassantSquare;

        bool valid =
            enPassantSquare is not null
            && move.To == enPassantSquare
            && move.IsDiagonal()
            && move.OffsetRank == game.TurnPlayer.Player.RankDirection()
            && !game.TryGetTargetPiece(move, out _);

        type = valid ? MoveType.EnPassant : null;

        return valid;
    }
}