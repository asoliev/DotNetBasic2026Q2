using System.Reflection;
using System.Runtime.Loader;
using Task1.Contracts;

namespace Task1;

internal static class ProviderRegistry
{
    private static readonly Lazy<IReadOnlyDictionary<ConfigurationProviderType, IConfigurationProvider>> providers = new(BuildProviders, true);

    public static IConfigurationProvider GetProvider(ConfigurationProviderType providerType)
    {
        if (!providers.Value.TryGetValue(providerType, out IConfigurationProvider? provider))
            throw new NotSupportedException($"No configuration provider plugin was found for '{providerType}'.");

        return provider;
    }

    private static IReadOnlyDictionary<ConfigurationProviderType, IConfigurationProvider> BuildProviders()
    {
        LoadPluginAssemblies();

        var discoveredProviders = new Dictionary<ConfigurationProviderType, IConfigurationProvider>();

        foreach (Type providerType in DiscoverProviderTypes())
        {
            ConfigurationProviderAttribute? metadata = providerType.GetCustomAttribute<ConfigurationProviderAttribute>();
            if (metadata is null)
                continue;

            if (Activator.CreateInstance(providerType) is not IConfigurationProvider provider)
                continue;

            discoveredProviders[metadata.ProviderType] = provider;
        }

        return discoveredProviders;
    }

    private static IEnumerable<Type> DiscoverProviderTypes()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = [.. ex.Types.Where(type => type is not null).Cast<Type>()];
            }

            foreach (Type type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                if (!typeof(IConfigurationProvider).IsAssignableFrom(type))
                    continue;

                yield return type;
            }
        }
    }

    private static void LoadPluginAssemblies()
    {
        foreach (string assemblyPath in Directory.EnumerateFiles(AppContext.BaseDirectory, "Task1.*ProviderPlugin.dll"))
        {
            try
            {
                AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));
            }
            catch (FileLoadException)
            {
            }
        }
    }
}