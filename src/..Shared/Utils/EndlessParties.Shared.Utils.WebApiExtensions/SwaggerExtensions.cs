using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления и настройки Swagger
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Добавление и настройка Swagger
    /// </summary>
    public static IServiceCollection AddSwagger(this IServiceCollection services, string title, string version)
    {
        var filePaths = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);

        services
            .AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version });

                foreach (var filePath in filePaths)
                {
                    setup.IncludeXmlComments(filePath, true);
                }

                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Укажите JWT-токен"
                };

                setup
                    .AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
                setup
                    .AddSecurityRequirement(document => new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
                    });
            });

        return services;
    }
}