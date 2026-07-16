namespace FileCabinet.Domain.Models;

public class Book : Document
{
    public required string Isbn { get; init; }
    public required string[] Authors { get; init; }
    public string Publisher { get; init; } = string.Empty;
    public int NumberOfPages { get; init; }

    public override string ToString() =>
        $"""
        [Book]
          Title     : {Title}
          ISBN      : {Isbn}
          Authors   : {string.Join(", ", Authors)}
          Publisher : {Publisher}
          Pages     : {NumberOfPages}
          Published : {DatePublished}
        """;
}
