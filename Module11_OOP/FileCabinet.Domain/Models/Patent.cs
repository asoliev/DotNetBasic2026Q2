namespace FileCabinet.Domain.Models;

public sealed class Patent : Document
{
    public required string[] Authors { get; init; }
    public required string UniqueId { get; init; }
    public DateOnly ExpirationDate { get; init; }

    public override string ToString() =>
        $"""
        [Patent]
          Title       : {Title}
          Unique ID   : {UniqueId}
          Authors     : {string.Join(", ", Authors)}
          Published   : {DatePublished}
          Expires     : {ExpirationDate}
        """;
}
