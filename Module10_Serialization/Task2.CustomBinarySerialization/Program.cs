#pragma warning disable SYSLIB0050

using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Task2.CustomBinarySerialization;

public static class Program
{
    public static void Main()
    {
        UserProfile profile = new("Ada", "Lovelace");
        string filePath = Path.Combine(AppContext.BaseDirectory, "profile.bin");

        CustomBinarySerializer.Serialize(profile, filePath);
        UserProfile restoredProfile = CustomBinarySerializer.Deserialize<UserProfile>(filePath);

        Console.WriteLine($"Binary file: {filePath}");
        Console.WriteLine($"Restored profile: {restoredProfile.FirstName} {restoredProfile.LastName}");
    }
}

[Serializable]
public sealed class UserProfile : ISerializable
{
    public string FirstName { get; }

    public string LastName { get; }

    public UserProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    private UserProfile(SerializationInfo info, StreamingContext context)
    {
        FirstName = info.GetString(nameof(FirstName)) ?? string.Empty;
        LastName = info.GetString(nameof(LastName)) ?? string.Empty;
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(FirstName), FirstName);
        info.AddValue(nameof(LastName), LastName);
    }
}

public static class CustomBinarySerializer
{
    public static void Serialize<T>(T value, string filePath) where T : ISerializable
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        SerializationInfo info = new(typeof(T), new FormatterConverter());
        value.GetObjectData(info, new(StreamingContextStates.File));

        using FileStream stream = File.Create(filePath);
        using BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: false);

        writer.Write(typeof(T).AssemblyQualifiedName ?? typeof(T).FullName ?? typeof(T).Name);
        writer.Write(info.MemberCount);

        foreach (SerializationEntry entry in info)
        {
            writer.Write(entry.Name ?? string.Empty);
            writer.Write(entry.Value as string ?? string.Empty);
        }
    }

    public static T Deserialize<T>(string filePath) where T : class, ISerializable
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using FileStream stream = File.OpenRead(filePath);
        using BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen: false);

        string typeName = reader.ReadString();
        Type type = Type.GetType(typeName, throwOnError: true)!;
        int memberCount = reader.ReadInt32();

        SerializationInfo info = new(type, new FormatterConverter());
        for (int index = 0; index < memberCount; index++)
        {
            string name = reader.ReadString();
            string value = reader.ReadString();
            info.AddValue(name, value);
        }

        ConstructorInfo? constructor = type.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(SerializationInfo), typeof(StreamingContext)],
            modifiers: null);

        if (constructor is null)
        {
            throw new InvalidOperationException($"Type {type.FullName} does not define the serialization constructor.");
        }

        return (T)constructor.Invoke([info, new StreamingContext(StreamingContextStates.File)]);
    }
}

#pragma warning restore SYSLIB0050