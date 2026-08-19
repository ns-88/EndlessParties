using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EndlessParties.Tests.Application.Fakes;

/// <summary>
/// Фейковая реализация <see cref="IExecutionStrategy"/>
/// </summary>
internal class FakeExecutionStrategy : IExecutionStrategy
{
    /// <inheritdoc />
    public bool RetriesOnFailure => true;


    /// <inheritdoc />
    public TResult Execute<TState, TResult>(TState state, Func<DbContext, TState, TResult> operation, Func<DbContext, TState, ExecutionResult<TResult>>? verifySucceeded)
    {
        return operation(null!, state);
    }

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<DbContext, TState, CancellationToken, Task<TResult>> operation, Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
        CancellationToken cancellationToken)
    {
        return operation(null!, state, cancellationToken);
    }
}