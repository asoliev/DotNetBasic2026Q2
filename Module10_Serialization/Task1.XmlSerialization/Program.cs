using System.Xml.Serialization;

namespace Task1.XmlSerialization;

public static class Program
{
    public static void Main()
    {
        Department department = new()
        {
            DepartmentName = "Platform Team",
            Employees =
            [
                new() { EmpoyeeName = "Margaret Hamilton" },
                new() { EmpoyeeName = "Katherine Johnson" }
            ]
        };

        string filePath = Path.Combine(AppContext.BaseDirectory, "department.xml");

        DepartmentXmlSerializer.Serialize(department, filePath);
        Department restoredDepartment = DepartmentXmlSerializer.Deserialize(filePath);

        Console.WriteLine($"XML file: {filePath}");
        Console.WriteLine($"Restored department: {restoredDepartment.DepartmentName}");
        foreach (Employee employee in restoredDepartment.Employees)
        {
            Console.WriteLine($"- {employee.EmpoyeeName}");
        }
    }
}

[XmlRoot("department")]
public sealed class Department
{
    [XmlAttribute("name")]
    public string DepartmentName { get; set; } = string.Empty;

    [XmlArray("employees")]
    [XmlArrayItem("employee")]
    public List<Employee> Employees { get; set; } = [];
}

public sealed class Employee
{
    [XmlAttribute("name")]
    public string EmpoyeeName { get; set; } = string.Empty;
}

public static class DepartmentXmlSerializer
{
    public static void Serialize(Department department, string filePath)
    {
        ArgumentNullException.ThrowIfNull(department);

        XmlSerializer serializer = new(typeof(Department));
        using FileStream stream = File.Create(filePath);
        serializer.Serialize(stream, department);
    }

    public static Department Deserialize(string filePath)
    {
        XmlSerializer serializer = new(typeof(Department));
        using FileStream stream = File.OpenRead(filePath);
        return (Department)serializer.Deserialize(stream)!;
    }
}