namespace FileCabinet.Infrastructure.Caching;

using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

/// <summary>
/// Decorator that adds per-document-type in-memory caching over any IDocumentRepository.
/// Cache key is documentNumber. Each document in a cached result is validated against
/// its own policy. If any document is un-cacheable or expired the full set is re-fetched.
/// </summary>
public sealed class CachedDocumentRepository(IDocumentRepository inner, DocumentCacheConfiguration config) : IDocumentRepository
{
    private readonly IDocumentRepository _inner = inner;
    private readonly DocumentCacheConfiguration _config = config;

    // number -> list of (document, cachedAt) — only documents whose policy.ShouldCache == true
    private readonly Dictionary<string, List<(Document document, DateTime cachedAt)>> _cache = [];

    public IReadOnlyList<Document> Search(string documentNumber)
    {
        if (_cache.TryGetValue(documentNumber, out List<(Document document, DateTime cachedAt)>? cached) && IsAllValid(cached))
        {
            Console.WriteLine($"  [cache hit] #{documentNumber}");
            return cached.Select(e => e.document).ToList();
        }

        Console.WriteLine($"  [cache miss] #{documentNumber}");
        IReadOnlyList<Document> fresh = _inner.Search(documentNumber);
        UpdateCache(documentNumber, fresh);
        return fresh;
    }

    private bool IsAllValid(List<(Document document, DateTime cachedAt)> entries)
    {
        foreach ((Document? document, DateTime cachedAt) in entries)
        {
            ICachePolicy policy = _config.GetPolicy(document.GetType());
            if (!policy.ShouldCache || policy.IsExpired(cachedAt))
                return false;
        }

        return entries.Count > 0;
    }

    private void UpdateCache(string documentNumber, IReadOnlyList<Document> documents)
    {
        var cacheable = documents
            .Where(d => _config.GetPolicy(d.GetType()).ShouldCache)
            .Select(d => (document: d, cachedAt: DateTime.UtcNow))
            .ToList();

        if (cacheable.Count > 0)
            _cache[documentNumber] = cacheable;
        else
            _cache.Remove(documentNumber);
    }
}
