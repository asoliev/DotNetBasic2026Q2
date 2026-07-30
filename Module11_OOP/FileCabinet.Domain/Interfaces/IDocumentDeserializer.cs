namespace FileCabinet.Domain.Interfaces;

using FileCabinet.Domain.Models;

public interface IDocumentDeserializer
{
    string DocumentType { get; }
    Document? Deserialize(string json);
}
