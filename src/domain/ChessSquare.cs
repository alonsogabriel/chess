using System.Diagnostics.CodeAnalysis;

namespace Chess.Domain;

public readonly record struct ChessSquare
{
    public ChessSquare() : this(0, 0) { }
    public ChessSquare(int file, int rank)
    {
        File = InRange(file);
        Rank = InRange(rank);
    }

    public static bool TryCreate(int file, int rank, [NotNullWhen(true)] out ChessSquare? square)
    {
        square = null;

        if (file < 0 || file + 1 > ChessBoard.TOTAL_FILES)
            return false;

        if (rank < 0 || rank + 1 > ChessBoard.TOTAL_RANKS)
            return false;

        square = new() { File = file, Rank = rank };

        return true;
    }

    public static ChessSquare? TryCreate(int file, int rank)
    {
        TryCreate(file, rank, out var square);
        return square;
    }

    public static ChessSquare From(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        if (value.Length != 2)
            return new();

        var file = (value[0] - 'A') % 32;
        var rank = ChessBoard.TOTAL_RANKS - (value[1] - '0');

        return new(file, rank);
    }
    public int File { get; private init; }
    public int Rank { get; private init; }

    public override string ToString()
    {
        var file = (char)('A' + File);
        var rank = (char)('8' - Rank);

        return new string([file, rank]);
    }
    private static int InRange(int value) => Math.Max(Math.Min(value, ChessBoard.TOTAL_FILES - 1), 0);
}