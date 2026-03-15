using System.Diagnostics.CodeAnalysis;
using Chess.Domain.Moves;

namespace Chess.Domain;

public class ChessGame
{
    private readonly ChessBoard _board;
    private readonly List<IChessGameEvent> _events = [];
    private ChessGamePlayer _turnPlayer;

    private static readonly Dictionary<ChessPiece, IMoveValidator> _moveValidators = new()
    {
        [ChessPiece.Pawn] = new PawnMoveValidator(),
        [ChessPiece.Knight] = new KnightMoveValidator(),
        [ChessPiece.Bishop] = new BishopMoveValidator(),
        [ChessPiece.Rook] = new RookMoveValidator(),
        [ChessPiece.Queen] = new QueenMoveValidator(),
        [ChessPiece.King] = new KingMoveValidator()
    };

    public ChessGame()
    {
        _board = ChessBoard.Standard();
        _board.Reset();
        Player1 = new PlayerInfo { Player = ChessGamePlayer.Player1, Name = nameof(Player1) };
        Player2 = new PlayerInfo { Player = ChessGamePlayer.Player2, Name = nameof(Player2) };
        _turnPlayer = ChessGamePlayer.Player1;
    }

    public PlayerInfo TurnPlayer => _turnPlayer.IsPlayer1() ? Player1 : Player2;
    public PlayerInfo WaitingPlayer => _turnPlayer.IsPlayer1() ? Player2 : Player1;
    public PlayerInfo Player1 { get; private set; }
    public PlayerInfo Player2 { get; private set; }

    public bool Move(MoveSpan move)
    {
        if (!TryGetTurnPlayerPiece(move.From, out var movedPiece))
            return false;

        if (TryGetTurnPlayerPiece(move.To, out _))
            return false;

        var validator = _moveValidators[movedPiece.Value.Piece];

        if (validator.MoveIsValid(move, this, out var type))
        {
            HandleValidMove(move, type.Value);
            return true;
        }

        return false;
    }

    private void HandleValidMove(MoveSpan move, MoveType type)
    {
        _board.TryMovePiece(move, out var movedPiece, out var targetPiece);
        if (type == MoveType.EnPassant)
        {
            // TODO Handle En passant => remove piece from en passant square and update
            var enPassantSquare = move.To.AddRanks(WaitingPlayer.Player.RankDirection());
            _board.RemovePiece(enPassantSquare, out targetPiece);
        }
        else if (type == MoveType.DoubleStep)
        {
            WaitingPlayer.AvailableEnPassantSquare = move.To.AddRanks(WaitingPlayer.Player.RankDirection());
        }
        TurnPlayer.AvailableEnPassantSquare = null;

        // TODO revert if move gets king in check
        var data = new MoveInfo
        {
            Move = move,
            Type = type,
            MovedPiece = movedPiece!.Value,
            CapturedPiece = targetPiece
        };

        Raise(new ChessGameMoveEvent(data));

        ChangeTurnPlayer();
    }

    private void Raise(IChessGameEvent @event)
    {
        _events.Add(@event);

        if (@event is ChessGameMoveEvent move)
        {
            OnMove?.Invoke(move);
        }
    }

    public bool TryGetPiece(ChessSquare square, [NotNullWhen(true)] out ChessGamePiece? piece)
    {
        return _board.TryGetPiece(square, out piece);
    }

    public bool TryGetMovedPiece(MoveSpan move, [NotNullWhen(true)] out ChessGamePiece? piece)
    {
        return _board.TryGetPiece(move.From, out piece);
    }

    public bool TryGetTargetPiece(MoveSpan move, [NotNullWhen(true)] out ChessGamePiece? piece)
    {
        return _board.TryGetPiece(move.To, out piece);
    }

    public bool TryGetTurnPlayerPiece(ChessSquare square, [NotNullWhen(true)] out ChessGamePiece? piece)
    {
        return _board.TryGetPiece(square, out piece) && piece.Value.Player == TurnPlayer.Player;
    }

    public bool PathIsBlocked(MoveSpan move)
    {
        return _board.PathIsBlocked(move);
    }

    public override string ToString()
    {
        return _board.ToString();
    }

    public ChessGameMoveAction? OnMove { get; set; }

    private void ChangeTurnPlayer()
    {
        _turnPlayer = _turnPlayer.IsPlayer1() ?
            ChessGamePlayer.Player2 :
            ChessGamePlayer.Player1;
    }
}

public delegate void ChessGameMoveAction(ChessGameMoveEvent @event);

public class MoveInfo
{
    public MoveSpan Move { get; internal init; }
    public ChessGamePiece MovedPiece { get; internal init; }
    public ChessGamePiece? CapturedPiece { get; internal init; }
    public MoveType Type { get; internal init; }
}

public interface IChessGameEvent
{
    Guid Id { get; }
}

public sealed class ChessGameMoveEvent(MoveInfo _data) : IChessGameEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public MoveInfo Data => _data;
}