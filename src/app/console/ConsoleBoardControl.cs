using Chess.Domain;

namespace Chess.Console;

public class ChessBoardControl
{
    private readonly SequentialControl control;

    public ChessBoardControl(ChessSquare initialSquare)
    {
        Square = initialSquare;
        control = CreateControl();
    }
    public ChessBoardControl() : this(new ChessSquare().AddRanks(3)) { }
    public bool Read()
    {
        control.Read();
        return true;
    }

    public ChessSquare Square { get; private set; }
    public ChessSquare? SelectedSquare { get; private set; }
    public bool IsOnSelectedSquare => SelectedSquare.HasValue && Square == SelectedSquare.Value;

    public ControlSquareChanged? OnChangeSquare { get; set; }
    public ControlSelectPressed? OnPressSelect { get; set; }
    public ControlMoveCompleted? OnCompleteMove { get; set; }

    private SequentialControl CreateControl()
    {
        var control = new SequentialControl(
            ChessBoard.TOTAL_RANKS,
            ChessBoard.TOTAL_FILES,
            Square.Rank,
            Square.File,
            new());

        control.OnInput += OnInput;
        control.OnSelect += OnSelect;

        return control;
    }
    public void UnselectSquare()
    {
        SelectedSquare = null;
    }
    private void OnInput(SequentialControl control, ConsoleKey key)
    {
        if (key == ConsoleKey.Escape)
        {
            SelectedSquare = null;
            return;
        }

        var lastSquare = Square;
        Square = new(control.X, control.Y);

        if (Square != lastSquare)
        {
            OnChangeSquare?.Invoke(lastSquare, Square);
        }
    }

    private void OnSelect(SequentialControl control)
    {
        if (!SelectedSquare.HasValue)
        {
            OnPressSelect?.Invoke((SelectedSquare = Square).Value);
            control.Read();
            return;
        }

        if (Square != SelectedSquare)
        {
            OnCompleteMove?.Invoke(new(SelectedSquare.Value, Square));
            UnselectSquare();
        }
    }
}

public delegate void ControlSquareChanged(ChessSquare previous, ChessSquare current);
public delegate void ControlSelectPressed(ChessSquare selectedSquare);
public delegate void ControlMoveCompleted(MoveSpan move);