using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;
using FileCabinet.Infrastructure.Caching;
using FileCabinet.Infrastructure.Deserializers;
using FileCabinet.Infrastructure.FileSystem;

// Composition root
var storagePath = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "data");

IDocumentDeserializer[] deserializers =
[
    new PatentDeserializer(),
    new BookDeserializer(),
    new LocalizedBookDeserializer(),
    new MagazineDeserializer(),
];

var cacheConfig = new DocumentCacheConfiguration()
    .WithPolicy<Patent>(new NeverExpirePolicy())
    .WithPolicy<Book>(new TimedExpiryPolicy(TimeSpan.FromMinutes(5)))
    .WithPolicy<LocalizedBook>(new NeverExpirePolicy())
    .WithPolicy<Magazine>(new NoCachePolicy());

var fileRepo = new FileSystemDocumentRepository(storagePath, deserializers);
var repository = new CachedDocumentRepository(fileRepo, cacheConfig);

Console.WriteLine($"File Cabinet Search  |  storage: {storagePath}");
Console.WriteLine("Type a document number to search, or 'q' to quit.\n");

while (true)
{
    Console.Write("Document number: ");
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input) || input.Equals("q", StringComparison.OrdinalIgnoreCase))
        break;

    var results = repository.Search(input);

    if (results.Count == 0)
    {
        Console.WriteLine("  No documents found.\n");
        continue;
    }

    foreach (var doc in results)
    {
        Console.WriteLine(doc.ToString());
        Console.WriteLine();
    }
}
