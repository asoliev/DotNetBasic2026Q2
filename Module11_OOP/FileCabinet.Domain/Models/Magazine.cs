namespace FileCabinet.Domain.Models;

public sealed class Magazine : Document
{
    public required string Publisher { get; init; }
    public int ReleaseNumber { get; init; }

    public override string ToString() =>
        $"""
        [Magazine]
          Title         : {Title}
          Publisher     : {Publisher}
          Release No.   : {ReleaseNumber}
          Published     : {DatePublished}
        """;
}
