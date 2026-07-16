namespace FileCabinet.Domain.Models;

public abstract class Document
{
    public required string Title { get; init; }
    public DateOnly DatePublished { get; init; }

    public abstract override string ToString();
}
