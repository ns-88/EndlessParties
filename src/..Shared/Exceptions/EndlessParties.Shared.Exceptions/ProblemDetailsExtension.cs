using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Shared.Exceptions;

/// <summary>
/// Набор методов-расширений для типа <see cref="ProblemDetails"/>
/// </summary>
internal static class ProblemDetailsExtension
{
    extension(ProblemDetails problemDetails)
    {
        /// <summary>
        /// Выполнение записи ошибки приложения в поток ответа
        /// </summary>
        public async Task<bool> TryWriteToResponse(HttpContext httpContext, CancellationToken cancellationToken)
        {
            try
            {
                httpContext.Response.ContentType = "application/problem+json";
                httpContext.Response.StatusCode = problemDetails.Status!.Value;

                await httpContext.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType(), cancellationToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}