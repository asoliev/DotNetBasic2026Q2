namespace FileCabinet.Infrastructure.Deserializers;

using System.Text.Json;
using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

public sealed class LocalizedBookDeserializer : IDocumentDeserializer
{
    public string DocumentType => "localizedbook";

    public Document? Deserialize(string json) =>
        JsonSerializer.Deserialize<LocalizedBook>(json, JsonOptions.Default);
}
