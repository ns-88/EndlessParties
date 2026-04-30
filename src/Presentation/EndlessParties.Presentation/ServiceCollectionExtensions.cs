using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using EndlessParties.Shared.Validation;
using Microsoft.OpenApi;

namespace EndlessParties.Presentation;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов презентационного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов презентационного слоя
    /// </summary>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddHealthChecks();

        services
            .AddApplicationValidation(typeof(Program).Assembly);

        services
            .AddEndpointsApiExplorer()
            .AddRouting(setup => setup.LowercaseUrls = true)
            .AddSwagger()
            .AddControllers()
            .AddJsonOptions(setup =>
            {
                ConfigureSerializerOptions(setup.JsonSerializerOptions);
            });

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return services;
    }

    /// <summary>
    /// Добавление и настройка Swagger
    /// </summary>
    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        var filePaths = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);

        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo { Title = "EndlessParties API V1", Version = "v1" });

            foreach (var filePath in filePaths)
            {
                setup.IncludeXmlComments(filePath, true);
            }
        });

        return services;
    }

    /// <summary>
    /// Конфигурирование Json-сериализации
    /// </summary>
    private static void ConfigureSerializerOptions(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.WriteIndented = true;
        options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
    }
}