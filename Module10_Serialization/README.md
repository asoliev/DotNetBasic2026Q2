# Module 10 Serialization Guide

This folder contains the completed work for the serialization assignment.

## What Was Added

The solution file [Module10_Serialization.slnx](Module10_Serialization.slnx) includes five console applications:

- [Task1.BinarySerialization](Task1.BinarySerialization/Task1.BinarySerialization.csproj)
- [Task1.XmlSerialization](Task1.XmlSerialization/Task1.XmlSerialization.csproj)
- [Task1.JsonSerialization](Task1.JsonSerialization/Task1.JsonSerialization.csproj)
- [Task2.CustomBinarySerialization](Task2.CustomBinarySerialization/Task2.CustomBinarySerialization.csproj)
- [Task3.DeepCloning](Task3.DeepCloning/Task3.DeepCloning.csproj)

## Task 1 Overview

Task 1 is split into three standalone console apps so each serialization format can be tested independently.

Each app defines the same domain model:

- `Employee` with the `EmpoyeeName` property
- `Department` with the `DepartmentName` property and a list of employees

The apps serialize a sample `Department` instance to a file and then deserialize it back.

Format-specific notes:

- Binary serialization uses a small custom binary writer/reader approach.
- XML serialization uses `XmlSerializer` and XML attributes to control the output shape.
- JSON serialization uses `System.Text.Json` and JSON attributes to rename and order fields.

## Task 2 Overview

Task 2 demonstrates custom binary serialization with `ISerializable`.

The sample class `UserProfile` has two properties and provides its own serialization logic by:

- implementing `GetObjectData`
- adding a constructor that accepts `SerializationInfo` and `StreamingContext`
- writing and reading a small binary payload with a helper serializer

## Task 3 Overview

Task 3 demonstrates deep cloning by serializing and deserializing the object graph.

The clone is independent from the original because the `Department` and `Employee` objects are round-tripped through JSON.

## How To Run

From the repository root, you can run any project directly. For example:

```bash
dotnet run --project Module10_Serialization/Task1.JsonSerialization/Task1.JsonSerialization.csproj
dotnet run --project Module10_Serialization/Task2.CustomBinarySerialization/Task2.CustomBinarySerialization.csproj
dotnet run --project Module10_Serialization/Task3.DeepCloning/Task3.DeepCloning.csproj
```

## What To Check

When you run the projects, verify the following:

1. Task 1 Binary writes a `.bin` file and reads back the department name and employee list.
2. Task 1 XML writes a `.xml` file and uses XML attributes to shape the output.
3. Task 1 JSON writes a `.json` file and shows renamed and ordered JSON properties.
4. Task 2 writes a custom binary file and restores the `UserProfile` values through `ISerializable`.
5. Task 3 prints different values for the original and cloned objects after the original is changed.

## Notes

- The assignment uses the existing repository naming style and net10.0 console projects.
- `Task3.DeepCloning` proves independence by changing the original object after cloning and printing both results.