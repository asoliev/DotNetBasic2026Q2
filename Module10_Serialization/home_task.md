# Serializtion

## Task1

Create an empty solution. Add three Console applications for three subtasks:

- Binary serialization
- XML serialization
- Json serialization

On all subtasks create class Employee with string EmpoyeeName and class Department with string DepartmentName and list of Employees.

On the Main method serialize a Department object, write the serialized object to the file, and deserialize it from the file.

Add some attributes to control how properties are serialized for XML and Json formats.

## Task2

Create a new Console application. Create one simple class with two properties. By implementing ISerializable provide your own binary serialization mechanism.

## Task3

Create a new Console application. Create Employee and Department classes from Task1 and implement Deep Cloning using serialization. Make sure that cloned objects are completely independent.
