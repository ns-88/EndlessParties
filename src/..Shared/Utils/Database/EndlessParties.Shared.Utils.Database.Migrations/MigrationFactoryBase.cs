using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EndlessParties.Shared.Utils.Database.Migrations;

/// <summary>
/// Фабрика создания контекста БД <typeparamref name="TContext"/> при миграции
/// </summary>
public class MigrationFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private readonly string _connectionString;


    /// <summary>
    /// Конструктор
    /// </summary>
    protected MigrationFactoryBase(string configurationPath)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionString = configuration.GetConnectionString(configurationPath) ??
                            throw new InvalidOperationException($"Строка подключения не найдена. Путь: \"{configurationPath}\"");
    }


    /// <inheritdoc />
    public TContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();

        optionsBuilder
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .LogTo(Console.WriteLine)
            .UseNpgsql(_connectionString, builder => builder.MigrationsAssembly(typeof(TContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention();

        var context = Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);

        if (context == null)
        {
            throw new InvalidOperationException($"Ошибка создания контекста БД. Наименование контекста: \"{typeof(TContext).Name}\"");
        }

        return (TContext)context;
    }
}