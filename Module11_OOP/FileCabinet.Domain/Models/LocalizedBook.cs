namespace FileCabinet.Domain.Models;

public sealed class LocalizedBook : Book
{
    public required string OriginalPublisher { get; init; }
    public required string CountryOfLocalization { get; init; }
    public required string LocalPublisher { get; init; }

    public override string ToString() =>
        $"""
        [Localized Book]
          Title               : {Title}
          ISBN                : {Isbn}
          Authors             : {string.Join(", ", Authors)}
          Original Publisher  : {OriginalPublisher}
          Local Publisher     : {LocalPublisher}
          Country             : {CountryOfLocalization}
          Pages               : {NumberOfPages}
          Published           : {DatePublished}
        """;
}
