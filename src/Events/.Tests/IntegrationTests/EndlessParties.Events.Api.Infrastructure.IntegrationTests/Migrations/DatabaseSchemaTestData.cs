using Xunit;

namespace EndlessParties.Events.Api.Infrastructure.IntegrationTests.Migrations;

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
            TableName = "events",
            Columns =
            [
                "id", "title", "total_seats", "available_seats", "description", 
                "start_at", "end_at", "description_search_vector", "title_search_vector"
            ],
            Constraints =
            [
                new ConstraintData
                {
                    Name = "ck_events_seats",
                    Expression = "(((available_seats >= 0) AND (total_seats >= 0) AND (available_seats <= total_seats)))"
                },
                new ConstraintData
                {
                    Name = "ck_events_dates",
                    Expression = "((end_at >= start_at))"
                }
            ],
            ForeignKeys = [],
            Indexes =
            [
                new IndexData
                {
                    Name = "pk_events",
                    IsUnique = true,
                    Columns = ["id"]
                },
                new IndexData
                {
                    Name = "ix_events_start_at",
                    IsUnique = false,
                    Columns = ["start_at"]
                },
                new IndexData
                {
                    Name = "ix_events_end_at",
                    IsUnique = false,
                    Columns = ["end_at"]
                },
                new IndexData
                {
                    Name = "ix_events_description_search_vector",
                    IsUnique = false,
                    Columns = ["description_search_vector"]
                },
                new IndexData
                {
                    Name = "ix_events_title_search_vector",
                    IsUnique = false,
                    Columns = ["title_search_vector"]
                }
            ]
        });
    }
}