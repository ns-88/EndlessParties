using EndlessParties.Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EndlessParties.Database;

/// <summary>
/// Фабрика создания <see cref="EventsDbContext"/> при миграции
/// </summary>
internal class MigrationFactory : IDesignTimeDbContextFactory<EventsDbContext>
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private readonly string _connectionString;


    /// <summary>
    /// Конструктор
    /// </summary>
    public MigrationFactory()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionString = configuration.GetConnectionString("Postgres:Events") ??
                            throw new InvalidOperationException("Строка подключения не найдена. Путь: \"Postgres:Events\"");
    }


    /// <inheritdoc />
    public EventsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventsDbContext>();

        optionsBuilder
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .LogTo(Console.WriteLine)
            .UseNpgsql(_connectionString, builder => builder.MigrationsAssembly(typeof(EventsDbContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention();

        return new EventsDbContext(optionsBuilder.Options);
    }
}