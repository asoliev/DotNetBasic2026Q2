namespace FileCabinet.Infrastructure.Deserializers;

using System.Text.Json;
using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

public sealed class MagazineDeserializer : IDocumentDeserializer
{
    public string DocumentType => "magazine";

    public Document? Deserialize(string json) =>
        JsonSerializer.Deserialize<Magazine>(json, JsonOptions.Default);
}
