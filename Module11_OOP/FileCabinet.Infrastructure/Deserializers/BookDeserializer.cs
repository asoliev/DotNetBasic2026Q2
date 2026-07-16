namespace FileCabinet.Infrastructure.Deserializers;

using System.Text.Json;
using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

public sealed class BookDeserializer : IDocumentDeserializer
{
    public string DocumentType => "book";

    public Document? Deserialize(string json) =>
        JsonSerializer.Deserialize<Book>(json, JsonOptions.Default);
}
