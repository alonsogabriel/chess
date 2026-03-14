using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Chess.Domain;

public class ChessBoard
{
    public readonly ChessGamePiece?[,] _pieces;

    public const int TOTAL_FILES = 8;
    public const int TOTAL_RANKS = 8;

    public ChessBoard(int files, int ranks)
    {
        if (files != TOTAL_FILES)
            throw new ArgumentException("Number of files not supported", nameof(files));

        if (ranks != TOTAL_RANKS)
            throw new ArgumentException("Number of ranks not supported", nameof(ranks));

        _pieces = new ChessGamePiece?[ranks, files];
    }

    public static ChessBoard Standard() => new(TOTAL_FILES, TOTAL_RANKS);

    public static ChessPiece[] StandardBackRankSet => [
        ChessPiece.Rook,
        ChessPiece.Knight,
        ChessPiece.Bishop,
        ChessPiece.Queen,
        ChessPiece.King,
        ChessPiece.Bishop,
        ChessPiece.Knight,
        ChessPiece.Rook,
    ];

    // TODO move to Standard Chess Game?
    public void Reset()
    {
        for (int rank = 0; rank < TOTAL_RANKS; rank++)
        {
            var player = rank < 2 ? ChessGamePlayer.Player2 : ChessGamePlayer.Player1;
            var pieces = new ChessPiece[TOTAL_FILES];
            var emptyRank = false;

            if (rank == 0 || rank == TOTAL_FILES - 1)
            {
                pieces = StandardBackRankSet;
            }
            else if (rank == 1 || rank == TOTAL_RANKS - 2)
            {
                Array.Fill(pieces, ChessPiece.Pawn);
            }
            else
            {
                emptyRank = true;
            }

            for (int file = 0; file < TOTAL_FILES; file++)
            {
                _pieces[rank, file] = emptyRank ? null : new(pieces[file], player);
            }
        }
    }

    public bool TryMovePiece(MoveSpan move, [NotNullWhen(true)] out ChessGamePiece? movedPiece, out ChessGamePiece? removedPiece)
    {
        removedPiece = null;

        if (!RemovePiece(move.From, out movedPiece))
            return false;

        PutPiece(movedPiece.Value, move.To, out removedPiece);

        return true;
    }

    public bool TryGetPiece(ChessSquare square, [NotNullWhen(true)] out ChessGamePiece? piece)
    {
        piece = _pieces[square.Rank, square.File];
        return piece is not null;
    }

    public bool PutPiece(ChessGamePiece piece, ChessSquare square, [NotNullWhen(true)] out ChessGamePiece? removedPiece)
    {
        var replaced = TryGetPiece(square, out removedPiece);
        _pieces[square.Rank, square.File] = piece;

        return replaced;
    }

    public bool RemovePiece(ChessSquare square, [NotNullWhen(true)] out ChessGamePiece? piece)
    {
        if (TryGetPiece(square, out piece))
        {
            _pieces[square.Rank, square.File] = null;
            return true;
        }

        return false;
    }

    public bool PathIsBlocked(MoveSpan move)
    {
        if (!move.IsOrthogonal() && !move.IsDiagonal())
            return false;

        var (rank, file) = move.Offset;
        var steps = Math.Max(Math.Abs(rank), Math.Abs(file));

        var dy = Math.Sign(rank);
        var dx = Math.Sign(file);

        for (int i = 1; i < steps; i++)
        {
            int x = move.From.File + dx * i;
            int y = move.From.Rank + dy * i;
            if (_pieces[y, x] is not null)
                return true;
        }

        return false;
    }

    public override string ToString()
    {
        var sb = new StringBuilder(TOTAL_FILES * TOTAL_RANKS * 3);
        for (int rank = 0; rank < TOTAL_RANKS; rank++)
        {
            for (int file = 0; file < TOTAL_FILES; file++)
            {
                sb.Append(_pieces[rank, file]?.ToString() ?? "--").Append(' ');
            }
            sb.Length--;
            sb.AppendLine();
        }

        return sb.ToString();
    }
}