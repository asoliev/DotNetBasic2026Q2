using System.Text;

namespace Task1.BinarySerialization;

public static class Program
{
    public static void Main()
    {
        Department department = new()
        {
            DepartmentName = "Research and Development",
            Employees =
            [
                new() { EmpoyeeName = "Ada Lovelace" },
                new() { EmpoyeeName = "Grace Hopper" }
            ]
        };

        string filePath = Path.Combine(AppContext.BaseDirectory, "department.bin");

        DepartmentBinarySerializer.Serialize(department, filePath);
        Department restoredDepartment = DepartmentBinarySerializer.Deserialize(filePath);

        Console.WriteLine($"Binary file: {filePath}");
        Console.WriteLine($"Restored department: {restoredDepartment.DepartmentName}");
        foreach (Employee employee in restoredDepartment.Employees)
        {
            Console.WriteLine($"- {employee.EmpoyeeName}");
        }
    }
}

[Serializable]
public sealed class Employee
{
    public string EmpoyeeName { get; set; } = string.Empty;
}

[Serializable]
public sealed class Department
{
    public string DepartmentName { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = [];
}

public static class DepartmentBinarySerializer
{
    public static void Serialize(Department department, string filePath)
    {
        ArgumentNullException.ThrowIfNull(department);

        using FileStream stream = File.Create(filePath);
        using BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: false);

        writer.Write(department.DepartmentName);
        writer.Write(department.Employees.Count);

        foreach (Employee employee in department.Employees)
        {
            writer.Write(employee.EmpoyeeName);
        }
    }

    public static Department Deserialize(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        using BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen: false);

        Department department = new()
        {
            DepartmentName = reader.ReadString(),
            Employees = []
        };

        int employeeCount = reader.ReadInt32();
        for (int index = 0; index < employeeCount; index++)
        {
            department.Employees.Add(new()
            {
                EmpoyeeName = reader.ReadString()
            });
        }

        return department;
    }
}
