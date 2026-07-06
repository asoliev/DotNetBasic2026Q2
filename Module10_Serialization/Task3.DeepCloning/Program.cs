using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task3.DeepCloning;

public static class Program
{
    public static void Main()
    {
        Department original = new()
        {
            DepartmentName = "Data Platform",
            Employees =
            [
                new() { EmpoyeeName = "Donald Knuth" },
                new() { EmpoyeeName = "Edsger Dijkstra" }
            ]
        };

        Department cloned = original.DeepClone();

        original.DepartmentName = "Changed Original Department";
        original.Employees[0].EmpoyeeName = "Changed Employee Name";

        Console.WriteLine("Original department:");
        Console.WriteLine(original.DepartmentName);
        foreach (Employee employee in original.Employees)
        {
            Console.WriteLine($"- {employee.EmpoyeeName}");
        }

        Console.WriteLine();
        Console.WriteLine("Cloned department:");
        Console.WriteLine(cloned.DepartmentName);
        foreach (Employee employee in cloned.Employees)
        {
            Console.WriteLine($"- {employee.EmpoyeeName}");
        }
    }
}

public sealed class Department
{
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = string.Empty;

    [JsonPropertyName("employees")]
    public List<Employee> Employees { get; set; } = [];

    public Department DeepClone()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = false
        };

        string json = JsonSerializer.Serialize(this, options);
        return JsonSerializer.Deserialize<Department>(json, options)!;
    }
}

public sealed class Employee
{
    [JsonPropertyName("employee_name")]
    public string EmpoyeeName { get; set; } = string.Empty;
}