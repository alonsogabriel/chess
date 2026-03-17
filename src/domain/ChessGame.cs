using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Chess.Domain.Moves;

namespace Chess.Domain;

public class ChessGame
{
    private readonly ChessBoard _board;
    private readonly List<IChessGameEvent> _events = [];
    private ChessGamePlayer _turnPlayer;

    public ChessGame()
    {
        _board = ChessBoard.Standard();
        _board.Reset();
        Player1 = new PlayerInfo { Player = ChessGamePlayer.Player1, Name = nameof(Player1) };
        Player2 = new PlayerInfo { Player = ChessGamePlayer.Player2, Name = nameof(Player2) };
        _turnPlayer = ChessGamePlayer.Player1;
        MapPieces();
    }

    private void MapPieces()
    {
        for (int i = 0; i < ChessBoard.TOTAL_RANKS; i++)
        {
            for (int j = 0; j < ChessBoard.TOTAL_FILES; j++)
            {
                var sq = new ChessSquare(j, i);
                if (_board.TryGetPiece(sq, out var piece))
                {
                    var player = piece.Value.Player.IsPlayer1() ?
                        Player1 : Player2;

                    player.MapPiece(sq, piece.Value.Piece);
                }
            }
        }
    }

    public PlayerInfo TurnPlayer => _turnPlayer.IsPlayer1() ? Player1 : Player2;
    public PlayerInfo WaitingPlayer => _turnPlayer.IsPlayer1() ? Player2 : Player1;
    public PlayerInfo Player1 { get; private set; }
    public PlayerInfo Player2 { get; private set; }

    public bool Move(MoveSpan move)
    {
        if (!_board.TryGetPiece(move.From, out var movedPiece))
            return false;

        if (MoveValidator.From(movedPiece.Value).IsValid(move, this, out var type))
        {
            HandleValidMove(move, type.Value);
            return true;
        }

        return false;
    }

    private void HandleValidMove(MoveSpan move, MoveType type)
    {
        if (!_board.TryMovePiece(move, out var movedPiece, out var targetPiece))
            return;

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

        TurnPlayer.RemovePiece(move.From);
        TurnPlayer.MapPiece(move.To, movedPiece.Value.Piece);
        TurnPlayer.AvailableEnPassantSquare = null;

        if (targetPiece.HasValue)
        {
            TurnPlayer.AddCapturedPiece(targetPiece.Value.Piece);
            WaitingPlayer.RemovePiece(move.To);
        }

        // TODO revert if move gets king in check
        var data = new MoveInfo
        {
            Move = move,
            Type = type,
            MovedPiece = movedPiece.Value,
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

    public bool PathIsBlocked(MoveSpan move)
    {
        return _board.PathIsBlocked(move);
    }

    public PlayerInfo GetPlayerInfo(ChessGamePlayer player)
    {
        if (!Enum.IsDefined(player))
            throw new InvalidEnumArgumentException();

        return player.IsPlayer1() ? Player1 : Player2;
    }

    public Dictionary<ChessSquare, ChessPiece> FindPiecesThatCanMoveToSquare(ChessSquare square, ChessGamePlayer player)
    {
        var pieces = GetPlayerInfo(player).PieceMapping.Where(map =>
        {
            var move = new MoveSpan(From: map.Key, To: square);
            return MoveValidator.From(map.Value).IsValid(move, this, out _);
        });

        return pieces.ToDictionary();
    }

    public Dictionary<ChessSquare, ChessPiece> FindPiecesAttackingSquare(ChessSquare square, ChessGamePlayer player)
    {
        var pieces = GetPlayerInfo(player).PieceMapping.Where(map =>
        {
            // TODO simulation/do not consider existing piece on target square
            return false;
        });

        return pieces.ToDictionary();
    }

    public override string ToString()
    {
        return _board.ToString();
    }

    public ChessGameMoveAction? OnMove { get; set; }

    private void ChangeTurnPlayer()
    {
        _turnPlayer = _turnPlayer.Adversary();
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