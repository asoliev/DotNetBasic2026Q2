using System.Data;
using AdoNetLibrary.Models;
using Microsoft.Data.SqlClient;

namespace AdoNetLibrary.Repositories;

public sealed class OrderRepository(string connectionString) : IOrderRepository
{
    private readonly SqlConnectionFactory connectionFactory = new(connectionString);

    public int Create(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            INSERT INTO dbo.Orders (ProductId, Status, CreatedDate, UpdatedDate)
            OUTPUT INSERTED.Id
            VALUES (@productId, @status, @createdDate, @updatedDate);
            """,
            connection
        );

        command.Parameters.AddWithValue("@productId", order.ProductId);
        command.Parameters.AddWithValue("@status", order.Status.ToString());
        command.Parameters.AddWithValue("@createdDate", order.CreatedDate);
        command.Parameters.AddWithValue("@updatedDate", order.UpdatedDate);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public Order? GetById(int id)
    {
        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            SELECT Id, ProductId, Status, CreatedDate, UpdatedDate
            FROM dbo.Orders
            WHERE Id = @id;
            """,
            connection
        );
        command.Parameters.AddWithValue("@id", id);

        using SqlDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return this.MapOrder(reader);
    }

    public bool Update(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            UPDATE dbo.Orders
            SET ProductId = @productId,
                Status = @status,
                CreatedDate = @createdDate,
                UpdatedDate = @updatedDate
            WHERE Id = @id;
            """,
            connection
        );

        command.Parameters.AddWithValue("@id", order.Id);
        command.Parameters.AddWithValue("@productId", order.ProductId);
        command.Parameters.AddWithValue("@status", order.Status.ToString());
        command.Parameters.AddWithValue("@createdDate", order.CreatedDate);
        command.Parameters.AddWithValue("@updatedDate", order.UpdatedDate);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            DELETE FROM dbo.Orders
            WHERE Id = @id;
            """,
            connection
        );
        command.Parameters.AddWithValue("@id", id);

        return command.ExecuteNonQuery() > 0;
    }

    public IReadOnlyCollection<Order> GetOrders(OrderFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new("dbo.usp_GetOrders", connection)
        {
            CommandType = CommandType.StoredProcedure,
        };

        AddFilterParameters(command, filter);

        using SqlDataReader reader = command.ExecuteReader();
        List<Order> orders = [];
        while (reader.Read())
        {
            orders.Add(this.MapOrder(reader));
        }

        return orders;
    }

    public int DeleteOrders(OrderFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlTransaction transaction = connection.BeginTransaction();

        using SqlCommand command = new("dbo.usp_DeleteOrders", connection, transaction)
        {
            CommandType = CommandType.StoredProcedure,
        };

        AddFilterParameters(command, filter);

        try
        {
            int deletedRows = command.ExecuteNonQuery();
            transaction.Commit();
            return deletedRows;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void AddFilterParameters(SqlCommand command, OrderFilter filter)
    {
        command.Parameters.AddWithValue("@month", (object?)filter.Month ?? DBNull.Value);
        command.Parameters.AddWithValue("@year", (object?)filter.Year ?? DBNull.Value);
        command.Parameters.AddWithValue("@status", (object?)filter.Status?.ToString() ?? DBNull.Value);
        command.Parameters.AddWithValue("@productId", (object?)filter.ProductId ?? DBNull.Value);
    }

    private Order MapOrder(SqlDataReader reader)
    {
        string statusString = reader.GetString(2);
        bool parsed = Enum.TryParse<OrderStatus>(statusString, ignoreCase: true, out OrderStatus status);
        if (!parsed)
        {
            throw new InvalidOperationException($"Unexpected status value '{statusString}' in Orders table.");
        }

        return new()
        {
            Id = reader.GetInt32(0),
            ProductId = reader.GetInt32(1),
            Status = status,
            CreatedDate = reader.GetDateTime(3),
            UpdatedDate = reader.GetDateTime(4),
        };
    }
}
