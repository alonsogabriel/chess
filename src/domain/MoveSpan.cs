namespace Chess.Domain;

public readonly record struct MoveSpan(ChessSquare From, ChessSquare To)
{
    public bool Moved => From != To;

    public int OffsetRank => To.Rank - From.Rank;
    public int OffsetFile => To.File - From.File;

    public (int rank, int file) Offset => (OffsetRank, OffsetFile);
    public (int rank, int file) OffsetAbsolute => (Math.Abs(OffsetRank), Math.Abs(OffsetFile));

    public static MoveSpan Create(string move)
    {
        var fromTo = move.Split("->");

        return new (ChessSquare.From(fromTo.First()), ChessSquare.From(fromTo.Last()));
    }
}

public static class Move
{
    public static ChessSquare From(int file, int rank) => new(file, rank);
    public static MoveSpan To(this ChessSquare from, int file, int rank) => new(from, new(file, rank));
}