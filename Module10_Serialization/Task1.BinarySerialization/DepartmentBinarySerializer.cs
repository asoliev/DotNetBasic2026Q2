using System.Text;

namespace Task1.BinarySerialization;

public static class DepartmentBinarySerializer
{
    public static void Serialize(Department department, string filePath)
    {
        ArgumentNullException.ThrowIfNull(department);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(department.Employees);

        string tempPath = Path.GetTempFileName();
        try
        {
            using (FileStream stream = File.Create(tempPath))
            using (BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: false))
            {
                WriteDepartment(writer, department);
            }

            File.Move(tempPath, filePath, overwrite: true);
        }
        catch
        {
            File.Delete(tempPath);
            throw;
        }
    }

    public static Department Deserialize(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using FileStream stream = File.OpenRead(filePath);
        using BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen: false);

        return ReadDepartment(reader);
    }

    private static void WriteDepartment(BinaryWriter writer, Department department)
    {
        ArgumentNullException.ThrowIfNull(department.DepartmentName);
        writer.Write(department.DepartmentName);

        writer.Write(department.Employees.Count);

        foreach (Employee employee in department.Employees)
        {
            ArgumentNullException.ThrowIfNull(employee);
            ArgumentNullException.ThrowIfNull(employee.EmployeeName);
            writer.Write(employee.EmployeeName);
        }
    }

    private static Department ReadDepartment(BinaryReader reader)
    {
        Department department = new()
        {
            DepartmentName = reader.ReadString(),
            Employees = []
        };

        int employeeCount = reader.ReadInt32();
        if (employeeCount < 0)
            throw new InvalidDataException($"Invalid employee count: {employeeCount}.");

        for (int index = 0; index < employeeCount; index++)
        {
            department.Employees.Add(new()
            {
                EmployeeName = reader.ReadString()
            });
        }

        return department;
    }
}
