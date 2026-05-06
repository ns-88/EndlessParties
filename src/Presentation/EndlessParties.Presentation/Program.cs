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
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();
        
        var application = builder.Build();
        application.ConfigureApp();

        await application.RunAsync();
    }
}