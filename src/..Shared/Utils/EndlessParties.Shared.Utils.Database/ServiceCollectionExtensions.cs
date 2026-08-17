using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Database.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Utils.Database;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления контекста базы данных
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление базы данных
    /// </summary>
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services, DatabaseSettings settings)
        where TContext : DbContext
    {
        services.AddDbContextPool<TContext>(dbContextSetup =>
        {
            dbContextSetup
                .UseNpgsql(settings.ConnectionString, npgsqlSetup =>
                {
                    npgsqlSetup
                        .EnableRetryOnFailure(settings.RetryReconnectDatabaseCount)
                        .MigrationsAssembly(typeof(TContext).Assembly.FullName);
                });

            dbContextSetup.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork, DefaultUnitOfWork<TContext>>();

        return services;
    }
}