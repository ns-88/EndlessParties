using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EndlessParties.Shared.Utils.IntegrationTests;

/// <summary>
/// Базовый класс интеграционных тестов
/// </summary>
public class BaseIntegrationTest<TContext, TFixture> : IClassFixture<TFixture>, IAsyncLifetime
    where TContext : DbContext
    where TFixture : PostgreSqlContainerFixture<TContext>
{
    /// <summary>
    /// Фикстура <typeparamref name="TFixture"/>
    /// </summary>
    protected readonly TFixture Fixture;

    /// <summary>
    /// Провайдер <see cref="IServiceProvider"/>
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Токен отмены
    /// </summary>
    protected CancellationToken TestCancellationToken => TestContext.Current.CancellationToken;


    /// <summary>
    /// Конструктор
    /// </summary>
    protected BaseIntegrationTest(TFixture fixture)
    {
        Fixture = fixture;
        ServiceProvider = fixture.ServiceProvider;
    }


    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        await Fixture.ResetDb();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}