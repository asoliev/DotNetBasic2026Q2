namespace FileCabinet.Domain.Interfaces;

public interface ICachePolicy
{
    bool ShouldCache { get; }
    bool IsExpired(DateTime cachedAt);
}
