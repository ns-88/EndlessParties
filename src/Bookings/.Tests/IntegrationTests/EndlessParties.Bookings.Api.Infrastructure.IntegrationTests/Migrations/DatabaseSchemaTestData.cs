using Xunit;

namespace EndlessParties.Bookings.Api.Infrastructure.IntegrationTests.Migrations;

/// <summary>
/// Тестовые данные для теста сравнения схемы базы данных
/// </summary>
public sealed class DatabaseSchemaTestData : TheoryData<DatabaseSchemaTestCase>
{
    /// <inheritdoc />
    public DatabaseSchemaTestData()
    {
        Add(new DatabaseSchemaTestCase
        {
            TableName = "bookings",
            Columns = ["id", "event_id", "status", "created_at", "processed_at", "user_id"],
            Constraints = [],
            ForeignKeys = [],
            Indexes =
            [
                new IndexData
                {
                    Name = "pk_bookings",
                    IsUnique = true,
                    Columns = ["id"]
                },
                new IndexData
                {
                    Name = "ix_bookings_processed_at",
                    IsUnique = false,
                    Columns = ["processed_at"]
                },
                new IndexData
                {
                    Name = "ix_bookings_event_id",
                    IsUnique = false,
                    Columns = ["event_id"]
                },
                new IndexData
                {
                    Name = "ix_bookings_created_at",
                    IsUnique = false,
                    Columns = ["created_at"]
                },
                new IndexData
                {
                    Name="ix_bookings_user_id",
                    IsUnique = false,
                    Columns = ["user_id"]
                }
            ]
        });
    }
}