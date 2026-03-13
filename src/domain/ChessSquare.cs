namespace Chess.Domain;

public readonly record struct ChessSquare
{
    public ChessSquare() : this(0, 0) { }
    public ChessSquare(int file, int rank)
    {
        File = InRange(file);
        Rank = InRange(rank);
    }

    public static ChessSquare From(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        if (value.Length != 2)
            return new();

        var file = (value[0] - 'A') % 32;
        var rank = ChessBoard.STANDARD_RANKS - (value[1] - '0');

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
    private static int InRange(int value) => Math.Max(Math.Min(value, ChessBoard.STANDARD_FILES - 1), 0);
}