using Serilog;

namespace EndlessParties.Presentation;

/// <summary>
/// Основной класс приложения
/// </summary>
public class Program
{
    /// <summary>
    /// Точка входа
    /// </summary>
    /// <param name="args">Аргументы командной строки</param>
    public static async Task Main(string[] args)
    {
        Log.AddBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureServices();

            var application = builder.Build();
            application.ConfigureApp();

            await application.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Ошибка запуска приложения");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}