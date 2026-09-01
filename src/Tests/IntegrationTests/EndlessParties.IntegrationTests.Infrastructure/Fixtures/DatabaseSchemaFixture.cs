using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using EndlessParties.Database.Database;
using EndlessParties.Shared.Utils.IntegrationTests;
using Npgsql;
using Xunit.Sdk;

namespace EndlessParties.IntegrationTests.Infrastructure.Fixtures;

/// <summary>
/// Фикстура для тестов схемы базы данных
/// </summary>
public class DatabaseSchemaFixture : PostgreSqlContainerFixture<EventsDbContext>
{
    /// <inheritdoc />
    public DatabaseSchemaFixture(IMessageSink messageSink) : base(messageSink)
    {
    }


    /// <summary>
    /// Получение схемы базы данных
    /// </summary>
    public async Task<DatabaseSchema> GetSchema(CancellationToken cancellationToken)
    {
        var connectionString = Container.GetConnectionString();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        using var databaseReader = new DatabaseReader(connection, SqlType.PostgreSql);
        databaseReader.Owner = "public";

        return await Task.Run(() => databaseReader.ReadAll(cancellationToken), cancellationToken);
    }
}