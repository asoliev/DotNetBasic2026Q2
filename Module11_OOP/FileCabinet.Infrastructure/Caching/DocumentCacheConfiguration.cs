namespace FileCabinet.Infrastructure.Caching;

using FileCabinet.Domain.Interfaces;
using FileCabinet.Domain.Models;

public sealed class DocumentCacheConfiguration
{
    private readonly Dictionary<Type, ICachePolicy> _policies = [];
    private ICachePolicy _defaultPolicy = new NoCachePolicy();

    public DocumentCacheConfiguration WithPolicy<TDocument>(ICachePolicy policy)
        where TDocument : Document
    {
        _policies[typeof(TDocument)] = policy;
        return this;
    }

    public DocumentCacheConfiguration WithDefault(ICachePolicy policy)
    {
        _defaultPolicy = policy;
        return this;
    }

    public ICachePolicy GetPolicy(Type documentType) =>
        _policies.TryGetValue(documentType, out var policy) ? policy : _defaultPolicy;
}
