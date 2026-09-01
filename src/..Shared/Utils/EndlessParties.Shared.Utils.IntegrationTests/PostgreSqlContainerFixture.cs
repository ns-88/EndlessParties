using EndlessParties.Shared.Utils.Database;
using EndlessParties.Shared.Utils.Database.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.Xunit;
using Xunit.Sdk;

namespace EndlessParties.Shared.Utils.IntegrationTests;

/// <summary>
/// Фикстура для получения контейнера <see cref="PostgreSqlContainer"/>
/// </summary>
public abstract class PostgreSqlContainerFixture<TContext> : ContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>
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
    /// Провайдер <see cref="IServiceProvider"/>
    /// </summary>
    public IServiceProvider ServiceProvider
    {
        get => field ?? throw new InvalidOperationException("Значение провайдера \"ServiceProvider\" не задано");
        private set;
    }


    /// <inheritdoc />
    protected PostgreSqlContainerFixture(IMessageSink messageSink) : base(messageSink)
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

    /// <summary>
    /// Регистрация зависимостей для теста
    /// </summary>
    protected virtual void ConfigureServices(ServiceCollection serviceCollection)
    {
    }

    /// <inheritdoc />
    protected override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        
        var serviceCollection = new ServiceCollection();

        var connectionString = Container.GetConnectionString();
        var databaseSettings = new DatabaseSettings
        {
            ConnectionString = connectionString,
            RetryReconnectDatabaseCount = 3
        };
        serviceCollection.AddDatabase<TContext>(databaseSettings, false, true);

        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();

        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            await using var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await context.Database.MigrateAsync();
        }

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