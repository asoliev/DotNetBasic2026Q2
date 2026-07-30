namespace FileCabinet.Infrastructure.Deserializers;

using System.Text.Json;
using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

public sealed class PatentDeserializer : IDocumentDeserializer
{
    public string DocumentType => "patent";

    public Document? Deserialize(string json) =>
        JsonSerializer.Deserialize<Patent>(json, JsonOptions.Default);
}
