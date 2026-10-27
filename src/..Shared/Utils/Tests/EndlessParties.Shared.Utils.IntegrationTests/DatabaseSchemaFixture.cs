using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit.Sdk;

namespace EndlessParties.Shared.Utils.IntegrationTests;

/// <summary>
/// Фикстура для тестов схемы базы данных
/// </summary>
public class DatabaseSchemaFixture<TContext> : PostgreSqlContainerFixture<TContext>
    where TContext : DbContext
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