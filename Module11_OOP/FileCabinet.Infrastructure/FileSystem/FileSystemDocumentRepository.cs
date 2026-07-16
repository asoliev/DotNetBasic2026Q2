namespace FileCabinet.Infrastructure.FileSystem;

using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

/// <summary>
/// Searches for document cards stored as JSON files on the local file system.
/// File naming convention: {type}_#{number}.json  e.g. book_#42.json
/// </summary>
public sealed class FileSystemDocumentRepository : IDocumentRepository
{
    private readonly string _storagePath;
    private readonly Dictionary<string, IDocumentDeserializer> _deserializers;

    public FileSystemDocumentRepository(
        string storagePath,
        IEnumerable<IDocumentDeserializer> deserializers)
    {
        _storagePath = storagePath;
        _deserializers = deserializers.ToDictionary(
            d => d.DocumentType,
            d => d,
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<Document> Search(string documentNumber)
    {
        if (!Directory.Exists(_storagePath))
            return [];

        var pattern = $"*_#{documentNumber}.json";
        var files = Directory.GetFiles(_storagePath, pattern);

        var results = new List<Document>();
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var separatorIndex = fileName.IndexOf("_#", StringComparison.Ordinal);
            if (separatorIndex < 0) continue;

            var docType = fileName[..separatorIndex];
            if (!_deserializers.TryGetValue(docType, out var deserializer)) continue;

            var json = File.ReadAllText(file);
            var document = deserializer.Deserialize(json);
            if (document is not null)
                results.Add(document);
        }

        return results;
    }
}
