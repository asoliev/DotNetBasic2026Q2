using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task1.JsonSerialization;

public static class Program
{
    public static void Main()
    {
        Department department = new()
        {
            DepartmentName = "Cloud Services",
            Employees =
            [
                new() { EmpoyeeName = "Alan Turing" },
                new() { EmpoyeeName = "Barbara Liskov" }
            ]
        };

        string filePath = Path.Combine(AppContext.BaseDirectory, "department.json");

        DepartmentJsonSerializer.Serialize(department, filePath);
        Department restoredDepartment = DepartmentJsonSerializer.Deserialize(filePath);

        Console.WriteLine($"JSON file: {filePath}");
        Console.WriteLine($"Restored department: {restoredDepartment.DepartmentName}");
        foreach (Employee employee in restoredDepartment.Employees)
        {
            Console.WriteLine($"- {employee.EmpoyeeName}");
        }
    }
}

public sealed class Department
{
    [JsonPropertyName("department_name")]
    [JsonPropertyOrder(1)]
    public string DepartmentName { get; set; } = string.Empty;

    [JsonPropertyName("employees")]
    [JsonPropertyOrder(2)]
    public List<Employee> Employees { get; set; } = [];

    [JsonIgnore]
    public int EmployeeCount => Employees.Count;
}

public sealed class Employee
{
    [JsonPropertyName("employee_name")]
    public string EmpoyeeName { get; set; } = string.Empty;
}

public static class DepartmentJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static void Serialize(Department department, string filePath)
    {
        ArgumentNullException.ThrowIfNull(department);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string json = JsonSerializer.Serialize(department, Options);
        File.WriteAllText(filePath, json);
    }

    public static Department Deserialize(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string json = File.ReadAllText(filePath);
        Department? department = JsonSerializer.Deserialize<Department>(json, Options);

        return department
            ?? throw new InvalidOperationException(
                $"Failed to deserialize Department from file: '{filePath}'.");
    }
}