using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace Chess.Domain;

public static class Utils
{
    public static bool IsPawn(this ChessPiece piece) => piece == ChessPiece.Pawn;
    public static bool IsKnight(this ChessPiece piece) => piece == ChessPiece.Knight;
    public static bool IsBishop(this ChessPiece piece) => piece == ChessPiece.Bishop;
    public static bool IsRook(this ChessPiece piece) => piece == ChessPiece.Rook;
    public static bool IsQueen(this ChessPiece piece) => piece == ChessPiece.Queen;
    public static bool IsKing(this ChessPiece piece) => piece == ChessPiece.King;
    public static bool IsPawn(this ChessGamePiece piece) => piece.Piece.IsPawn();
    public static bool IsKnight(this ChessGamePiece piece) => piece.Piece.IsKnight();
    public static bool IsBishop(this ChessGamePiece piece) => piece.Piece.IsBishop();
    public static bool IsRook(this ChessGamePiece piece) => piece.Piece.IsRook();
    public static bool IsQueen(this ChessGamePiece piece) => piece.Piece.IsQueen();
    public static bool IsKing(this ChessGamePiece piece) => piece.Piece.IsKing();
    public static char Symbol(this ChessPiece piece, bool solid = true)
    {
        int kingCode = solid ? 0x265A : 0x2654;

        return (char)(kingCode + piece);
    }
    public static char Letter(this ChessPiece piece)
    {
        if (piece.IsKnight())
            return 'N';

        return Enum.GetName(piece)?.First() ?? '?';
    }

    public static bool IsLShaped(this MoveSpan move)
    {
        var (rank, file) = move.OffsetAbsolute;

        return rank == 2 && file == 1 || rank == 1 && file == 2;
    }

    public static bool IsVertical(this MoveSpan move)
    {
        var (rank, file) = move.Offset;

        return file == 0 && rank != 0;
    }

    public static bool IsHorizontal(this MoveSpan move)
    {
        var (rank, file) = move.Offset;

        return file != 0 && rank == 0;
    }

    public static bool IsDiagonal(this MoveSpan move)
    {
        var (rank, file) = move.OffsetAbsolute;

        return rank == file;
    }

    public static bool IsOrthogonal(this MoveSpan move)
    {
        return move.IsVertical() || move.IsHorizontal();
    }

    public static bool IsOrthogonalOrDiagonal(this MoveSpan move)
    {
        return move.IsOrthogonal() || move.IsDiagonal();
    }

    public static bool IsPlayer1(this ChessGamePlayer player) => player == ChessGamePlayer.Player1;
    public static bool IsPlayer2(this ChessGamePlayer player) => player == ChessGamePlayer.Player2;
    public static int RankDirection(this ChessGamePlayer player)
    {
        if (player.IsPlayer1())
            return -1;

        if (player.IsPlayer2())
            return 1;

        return 0;
    }

    public static int PawnRank(this ChessGamePlayer player)
    {
        if (player.IsPlayer1())
            return ChessBoard.TOTAL_RANKS - 2;

        if (player.IsPlayer2())
            return 1;

        return -1;
    }

    public static int BackRank(this ChessGamePlayer player)
    {
        if (player.IsPlayer1())
            return ChessBoard.TOTAL_RANKS - 1;

        if (player.IsPlayer2())
            return 0;

        return -1;
    }

    public static ChessSquare AddRanks(this ChessSquare square, int ranks)
    {
        return square.Add(0, ranks);
    }

    public static ChessSquare AddFiles(this ChessSquare square, int files)
    {
        return square.Add(files, 0);
    }

    public static ChessSquare Add(this ChessSquare square, int files, int ranks)
    {
        return new(square.File + files, square.Rank + ranks);
    }

    public static ChessGamePlayer Adversary(this ChessGamePlayer player)
    {
        if (!Enum.IsDefined(player))
            throw new InvalidEnumArgumentException();

        return player.IsPlayer1() ? ChessGamePlayer.Player2 : ChessGamePlayer.Player1;
    }

    public static bool TryGetOwnedPieceAndValidTarget(this ChessGame game, MoveSpan move,
        [NotNullWhen(true)] out ChessGamePiece? moved, out ChessGamePiece? target)
    {
        target = null;
        
        if (!game.TryGetOwnedPiece(move.From, out moved))
            return false;

        if (game.TryGetPiece(move.To, out target) &&
            target.Value.Player == game.TurnPlayer.Player)
            return false;

        return true;
    }

    public static bool TryGetOwnedPiece(this ChessGame game, ChessSquare square,
        [NotNullWhen(true)] out ChessGamePiece? piece)
    {
        if (game.TryGetPiece(square, out piece) && piece.Value.Player == game.TurnPlayer.Player)
        {
            return true;
        }

        piece = null;

        return false;
    }
}