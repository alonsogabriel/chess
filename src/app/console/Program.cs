using System.ComponentModel;
using System.Text.Json;
using Chess.Console;
using Chess.Domain;
using Chess.Domain.Moves;

await Tests.TestGame();

public static class Tests
{
    public static void TestBoard()
    {
        var board = ChessBoard.Standard();
        board.Reset();
        System.Console.WriteLine(board);
        if (board.TryMovePiece(Move.From(7, 6).To(0, 1), out var moved, out var removed))
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
                return;

            var move = MoveSpan.Create(input);

            if (board.TryMovePiece(move, out _, out _))
            {
                System.Console.WriteLine(board);
            }
        }
    }

    public static async Task TestGame()
    {
        System.Console.Clear();
        System.Console.CursorVisible = false;

        var boardOptions = await new ConsoleChessOptionsInitializer().Initialize();

        System.Console.Clear();

        var game = new ChessGame();
        var boardConsole = new ConsoleChessBoard(game, boardOptions);
        var control = new ChessBoardControl();

        game.OnMove += (ev) =>
        {
            if (ev.Data.Type == MoveType.EnPassant)
            {
                var enPassantSquare = ev.Data.Move.To.AddRanks(game.WaitingPlayer.Player.RankDirection());
                boardConsole.WriteSquare(enPassantSquare);
            }
        };

        control.OnChangeSquare += (previous, current) =>
        {
            if (previous != control.SelectedSquare)
            {
                boardConsole.WriteSquare(previous);
            }
            if (current != control.SelectedSquare)
            {
                boardConsole.WriteSquare(current, ChessBoardSquareAppearence.Highlighted);
            }
        };

        control.OnPressSelect += (selected) =>
        {
            if (!game.TryGetTurnPlayerPiece(selected, out _))
            {
                control.UnselectSquare();
                return;
            }

            boardConsole.WriteSquare(selected, ChessBoardSquareAppearence.Selected);
        };

        control.OnCompleteMove += (move) =>
        {
            if (game.Move(move))
            {
                boardConsole.WriteSquare(move.To);
            }
            boardConsole.WriteSquare(move.From);
        };

        boardConsole.Write();

        while (true)
        {
            if (control.Read())
            {
            }
        }
    }
}

public enum ChessBoardColorComponent
{
    Square1,
    Square2,
    Player1,
    Player2,
    SelectedSquare,
    HighlightedSquare
}

