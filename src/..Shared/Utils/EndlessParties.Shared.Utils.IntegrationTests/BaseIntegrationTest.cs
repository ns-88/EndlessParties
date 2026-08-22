using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EndlessParties.Shared.Utils.IntegrationTests;

/// <summary>
/// Базовый класс интеграционных тестов
/// </summary>
public class BaseIntegrationTest<TContext> : IClassFixture<PostgreSqlContainerFixture<TContext>>, IAsyncLifetime
    where TContext : DbContext
{
    /// <summary>
    /// Фикстура <see cref="PostgreSqlContainerFixture{T}"/>
    /// </summary>
    private readonly PostgreSqlContainerFixture<TContext> _fixture;

    /// <summary>
    /// Фабрика <see cref="IDbContextFactory{T}"/>
    /// </summary>
    protected IDbContextFactory<TContext> DbContextFactory { get; }

    /// <summary>
    /// Токен отмены
    /// </summary>
    protected CancellationToken TestCancellationToken => TestContext.Current.CancellationToken;


    /// <summary>
    /// Конструктор
    /// </summary>
    protected BaseIntegrationTest(PostgreSqlContainerFixture<TContext> fixture)
    {
        _fixture = fixture;
        DbContextFactory = fixture.DbContextFactory;
    }


    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        await _fixture.ResetDb();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}