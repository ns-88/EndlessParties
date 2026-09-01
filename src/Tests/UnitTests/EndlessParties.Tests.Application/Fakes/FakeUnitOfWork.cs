using EndlessParties.Shared.Utils.Database.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace EndlessParties.UnitTests.Application.Fakes;

/// <summary>
/// Фейковая реализация <see cref="IUnitOfWork"/>
/// </summary>
internal class FakeUnitOfWork : IUnitOfWork, IDisposable
{
    /// <summary>
    /// Семафор
    /// </summary>
    private readonly SemaphoreSlim _semaphore;


    /// <summary>
    /// Конструктор
    /// </summary>
    public FakeUnitOfWork()
    {
        _semaphore = new SemaphoreSlim(1, 1);
    }


    /// <inheritdoc />
    public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        var mock = new Mock<IDbContextTransaction>();

        mock
            .Setup(x => x.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);

        mock
            .Setup(x => x.DisposeAsync())
            .Callback(() => _semaphore.Release());

        return mock.Object;
    }

    /// <inheritdoc />
    public IExecutionStrategy CreateExecutionStrategy()
    {
        return new FakeExecutionStrategy();
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _semaphore.Dispose();
    }
}