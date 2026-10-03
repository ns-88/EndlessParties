using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using EndlessParties.Events.Api.Settings;
using EndlessParties.Shared.Utils.HttpUserContext;
using EndlessParties.Shared.Utils.WebApiExtensions;

namespace EndlessParties.Events.Api;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов презентационного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов презентационного слоя
    /// </summary>
    public static IServiceCollection AddPresentation(this IServiceCollection services, PresentationSettings settings)
    {
        settings.Validate();

        services
            .AddAuthentication(settings.Identity)
            .AddHealthChecks();

        services
            .AddHttpUserContextAccessor()
            .AddEndpointsApiExplorer()
            .AddRouting(setup => setup.LowercaseUrls = true)
            .AddSwagger("Events API V1", "v1")
            .AddControllers()
            .AddJsonOptions(setup =>
            {
                ConfigureSerializerOptions(setup.JsonSerializerOptions);
            });

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return services;
    }

    /// <summary>
    /// Конфигурирование Json-сериализации
    /// </summary>
    private static void ConfigureSerializerOptions(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.WriteIndented = true;
        options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
    }
}