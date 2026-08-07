namespace FileCabinet.Infrastructure.Caching;

using FileCabinet.Domain.Interfaces;

public sealed class NoCachePolicy : ICachePolicy
{
    public bool ShouldCache => false;
    public bool IsExpired(DateTime cachedAt) => true;
}

public sealed class NeverExpirePolicy : ICachePolicy
{
    public bool ShouldCache => true;
    public bool IsExpired(DateTime cachedAt) => false;
}

public sealed class TimedExpiryPolicy(TimeSpan ttl) : ICachePolicy
{
    private readonly TimeSpan _ttl = ttl;

    public bool ShouldCache => true;
    public bool IsExpired(DateTime cachedAt) => DateTime.UtcNow - cachedAt > _ttl;
}
