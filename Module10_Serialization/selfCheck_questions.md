# Module 10 Serialization Self-Check Answers

## 1. What is serialization?

Serialization is the process of converting an object into a format that can be stored, transmitted, or reconstructed later. The output can be binary, XML, JSON, or another format. Deserialization is the reverse process: turning the stored data back into an object.

## 2. Give examples where serialization is used

Serialization is used when data must move between memory, files, and network boundaries. Common examples include:

- Saving application settings or user data to a file
- Sending data between client and server in web applications or APIs
- Caching objects in memory or on disk
- Storing messages in queues or event streams
- Cloning objects by serializing and deserializing them

## 3. When should binary/XML/JSON serialization be used? What are the main advantages of each?

Binary serialization is usually used when compact storage and faster read/write operations are important. Its main advantages are a small payload size and efficient processing. It is best for internal use when both sides understand the same data format.

XML serialization is used when the data needs to be human-readable, easy to inspect, and compatible with systems that already use XML. Its main advantages are readability, self-describing structure, and good interoperability with older enterprise systems.

JSON serialization is used for web APIs, configuration files, and modern data exchange. Its main advantages are simplicity, readability, smaller size than XML, and strong support in modern frameworks and JavaScript-based clients.

In practice, JSON is the most common choice for APIs, XML is useful for legacy or document-oriented integrations, and binary is useful when performance and size matter more than readability.

## 4. When is custom serialization needed? How can custom serialization be implemented?

Custom serialization is needed when the default serializer cannot handle a type correctly or when you need full control over the serialized data. Typical cases include:

- Excluding sensitive or temporary fields
- Converting derived values instead of storing raw ones
- Supporting backward compatibility with old data formats
- Serializing objects that contain non-serializable members
- Saving data in a special binary or compact format

Custom serialization can be implemented in several ways:

- By implementing `ISerializable` and providing `GetObjectData` plus a special constructor that reads from `SerializationInfo`
- By using custom helper methods that write and read fields manually
- By applying serializer-specific attributes or converters, such as JSON converters or XML attributes

In this module, custom binary serialization is demonstrated with `ISerializable`, where the class controls exactly which values are written and how they are restored.
