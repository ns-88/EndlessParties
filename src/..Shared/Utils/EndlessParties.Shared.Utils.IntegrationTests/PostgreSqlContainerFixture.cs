using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.Xunit;
using Xunit.Sdk;

namespace EndlessParties.Shared.Utils.IntegrationTests;

/// <summary>
/// Фикстура для получения контейнера <see cref="PostgreSqlContainer"/>
/// </summary>
public class PostgreSqlContainerFixture<TContext> : ContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>
    where TContext : DbContext
{
    /// <summary>
    /// Сервис очистки БД <see cref="Respawn.Respawner"/>
    /// </summary>
    private Respawner Respawner
    {
        get => field ?? throw new InvalidOperationException("Значение сервиса очистки БД \"Respawner\" не задано");
        set;
    }

    /// <summary>
    /// Фабрика <see cref="IDbContextFactory{T}"/>
    /// </summary>
    public IDbContextFactory<TContext> DbContextFactory
    {
        get => field ?? throw new InvalidOperationException("Значение фабрики \"DbContextFactory\" не задано");
        private set;
    }


    /// <inheritdoc />
    public PostgreSqlContainerFixture(IMessageSink messageSink) : base(messageSink)
    {
    }


    /// <summary>
    /// Очистка тестовой БД
    /// </summary>
    public async Task ResetDb()
    {
        var connectionString = Container.GetConnectionString();
        await using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        await Respawner.ResetAsync(connection);
    }

    /// <inheritdoc />
    protected override PostgreSqlBuilder Configure()
    {
        var builder = base.Configure();

        return builder
            .WithImage("postgres:16-alpine")
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("test_password");
    }

    /// <inheritdoc />
    protected override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        var connectionString = Container.GetConnectionString();
        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        DbContextFactory = new PooledDbContextFactory<TContext>(options, 1);

        await using var context = await DbContextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();

        await using var connection = new NpgsqlConnection(connectionString);
        var respawnerOptions = new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        };

        await connection.OpenAsync();

        Respawner = await Respawner.CreateAsync(connection, respawnerOptions);
    }
}