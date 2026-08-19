using Moq;
using Moq.AutoMock;

namespace EndlessParties.Tests.Application.Infrastructure;

/// <summary>
/// Набор методов-расширений для класса <see cref="AutoMocker"/>
/// </summary>
internal static class AutoMockerExtensions
{
    /// <summary>
    /// Проверка наличия неверифицированных вызовов
    /// </summary>
    public static void VerifyNoOtherCalls(this AutoMocker mocker)
    {
        foreach (var pair in mocker.ResolvedObjects)
        {
            if (pair.Value is not Mock mock)
            {
                continue;
            }

            var method = mock.GetType().GetMethod("VerifyNoOtherCalls");

            method?.Invoke(mock, []);
        }
    }
}