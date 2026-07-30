namespace FileCabinet.Domain.Interfaces;

using FileCabinet.Domain.Models;

public interface IDocumentRepository
{
    IReadOnlyList<Document> Search(string documentNumber);
}
