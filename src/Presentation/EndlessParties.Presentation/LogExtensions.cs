using Serilog;

namespace EndlessParties.Presentation;

/// <summary>
/// Набор методов-расширений для класса <see cref="Log"/>
/// </summary>
internal static class LogExtensions
{
    extension(Log)
    {
        /// <summary>
        /// Добавление базового логгера
        /// </summary>
        public static void AddBootstrapLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateBootstrapLogger();
        }
    }
}