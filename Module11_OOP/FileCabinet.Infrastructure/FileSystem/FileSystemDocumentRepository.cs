namespace FileCabinet.Infrastructure.FileSystem;

using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

/// <summary>
/// Searches for document cards stored as JSON files on the local file system.
/// File naming convention: {type}_#{number}.json  e.g. book_#42.json
/// </summary>
public sealed class FileSystemDocumentRepository(
    string storagePath,
    IEnumerable<IDocumentDeserializer> deserializers) : IDocumentRepository
{
    private readonly string _storagePath = storagePath;
    private readonly Dictionary<string, IDocumentDeserializer> _deserializers = deserializers.ToDictionary(
            d => d.DocumentType,
            d => d,
            StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<Document> Search(string documentNumber)
    {
        if (!Directory.Exists(_storagePath))
            return [];

        string pattern = $"*_#{documentNumber}.json";
        string[] files = Directory.GetFiles(_storagePath, pattern);

        var results = new List<Document>();
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            int separatorIndex = fileName.IndexOf("_#", StringComparison.Ordinal);
            if (separatorIndex < 0) continue;

            string docType = fileName[..separatorIndex];
            if (!_deserializers.TryGetValue(docType, out IDocumentDeserializer? deserializer)) continue;

            string json = File.ReadAllText(file);
            Document? document = deserializer.Deserialize(json);
            if (document is not null)
                results.Add(document);
        }

        return results;
    }
}
