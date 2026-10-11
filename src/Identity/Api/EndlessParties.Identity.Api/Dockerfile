FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Identity/Api/EndlessParties.Identity.Api/EndlessParties.Identity.Api.csproj", "src/Identity/Api/EndlessParties.Identity.Api/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions/EndlessParties.Shared.Exceptions.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Models/EndlessParties.Shared.Exceptions.Models.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Models/"]
COPY ["src/..Shared/Utils/EndlessParties.Shared.Utils.WebApiExtensions/EndlessParties.Shared.Utils.WebApiExtensions.csproj", "src/..Shared/Utils/EndlessParties.Shared.Utils.WebApiExtensions/"]
COPY ["src/..Shared/Validations/EndlessParties.Shared.Validations/EndlessParties.Shared.Validations.csproj", "src/..Shared/Validations/EndlessParties.Shared.Validations/"]
COPY ["src/Identity/.Shared/Database/EndlessParties.Identity.Database/EndlessParties.Identity.Database.csproj", "src/Identity/.Shared/Database/EndlessParties.Identity.Database/"]
COPY ["src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database.Migrations/EndlessParties.Shared.Utils.Database.Migrations.csproj", "src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database.Migrations/"]
COPY ["src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database/EndlessParties.Shared.Utils.Database.csproj", "src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database/"]
COPY ["src/Identity/.Shared/Domain/EndlessParties.Identity.Domain/EndlessParties.Identity.Domain.csproj", "src/Identity/.Shared/Domain/EndlessParties.Identity.Domain/"]
COPY ["src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.UserContext.Abstractions/EndlessParties.Shared.Utils.UserContext.Abstractions.csproj", "src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.UserContext.Abstractions/"]
COPY ["src/Identity/Api/EndlessParties.Identity.Api.App/EndlessParties.Identity.Api.App.csproj", "src/Identity/Api/EndlessParties.Identity.Api.App/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Extensions/EndlessParties.Shared.Exceptions.Extensions.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Extensions/"]
COPY ["src/Identity/.Shared/Repositories/EndlessParties.Identity.Repositories.Abstractions/EndlessParties.Identity.Repositories.Abstractions.csproj", "src/Identity/.Shared/Repositories/EndlessParties.Identity.Repositories.Abstractions/"]
COPY ["src/Identity/.Shared/Services/Cryptography/EndlessParties.Identity.Cryptography.Abstractions/EndlessParties.Identity.Cryptography.Abstractions.csproj", "src/Identity/.Shared/Services/Cryptography/EndlessParties.Identity.Cryptography.Abstractions/"]
COPY ["src/Identity/Api/EndlessParties.Identity.Api.Infrastructure/EndlessParties.Identity.Api.Infrastructure.csproj", "src/Identity/Api/EndlessParties.Identity.Api.Infrastructure/"]
COPY ["src/Identity/.Shared/Repositories/EndlessParties.Identity.Repositories/EndlessParties.Identity.Repositories.csproj", "src/Identity/.Shared/Repositories/EndlessParties.Identity.Repositories/"]
COPY ["src/Identity/.Shared/Services/Cryptography/EndlessParties.Identity.Cryptography/EndlessParties.Identity.Cryptography.csproj", "src/Identity/.Shared/Services/Cryptography/EndlessParties.Identity.Cryptography/"]
RUN dotnet restore "./src/Identity/Api/EndlessParties.Identity.Api/EndlessParties.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/Identity/Api/EndlessParties.Identity.Api"
RUN dotnet build "./EndlessParties.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EndlessParties.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EndlessParties.Identity.Api.dll"]