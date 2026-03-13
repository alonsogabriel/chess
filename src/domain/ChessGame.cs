namespace Chess.Domain;

public class ChessGame
{
    private readonly ChessBoard _board;

    public ChessGame()
    {
        _board = ChessBoard.Standard();
        _board.Reset();
    }
}

// public class PlayerInfo
// {
//     public string Name { get; set; }
// }
