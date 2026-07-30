namespace FileCabinet.Infrastructure;

using System.Text.Json;

internal static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}
