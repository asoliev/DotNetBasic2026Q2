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
                new() { EmployeeName = "Ada Lovelace" },
                new() { EmployeeName = "Grace Hopper" }
            ]
        };

        string filePath = Path.Combine(AppContext.BaseDirectory, "department.bin");

        DepartmentBinarySerializer.Serialize(department, filePath);
        Department restoredDepartment = DepartmentBinarySerializer.Deserialize(filePath);

        Console.WriteLine($"Binary file: {filePath}");
        Console.WriteLine($"Restored department: {restoredDepartment.DepartmentName}");
        foreach (Employee employee in restoredDepartment.Employees)
        {
            Console.WriteLine($"- {employee.EmployeeName}");
        }
    }
}

[Serializable]
public sealed class Employee
{
    public string EmployeeName { get; set; } = string.Empty;
}

[Serializable]
public sealed class Department
{
    public string DepartmentName { get; set; } = string.Empty;

    public List<Employee> Employees { get; set; } = [];
}
