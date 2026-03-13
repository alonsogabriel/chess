using Chess.Domain;

var board = ChessBoard.Standard();
board.Reset();
System.Console.WriteLine(board);
if (board.TryMovePiece(Move.From(7,6).To(0,1), out var moved, out var removed))
{
    System.Console.WriteLine(moved);
    System.Console.WriteLine(removed);

    System.Console.WriteLine(board);
}
else
{
    System.Console.WriteLine("Move failed");
}

while (true)
{
    var input = Console.ReadLine() ?? string.Empty;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        return 0;

    var move = MoveSpan.Create(input);

    if (board.TryMovePiece(move, out _, out _))
    {
        System.Console.WriteLine(board);
    }
}

