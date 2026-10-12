FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Events/Api/EndlessParties.Events.Api/EndlessParties.Events.Api.csproj", "src/Events/Api/EndlessParties.Events.Api/"]
COPY ["src/..Shared/Contracts/EndlessParties.Shared.Contracts.Bookings/EndlessParties.Shared.Contracts.Bookings.csproj", "src/..Shared/Contracts/EndlessParties.Shared.Contracts.Bookings/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions/EndlessParties.Shared.Exceptions.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Models/EndlessParties.Shared.Exceptions.Models.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Models/"]
COPY ["src/..Shared/Utils/EndlessParties.Shared.Utils.WebApiExtensions/EndlessParties.Shared.Utils.WebApiExtensions.csproj", "src/..Shared/Utils/EndlessParties.Shared.Utils.WebApiExtensions/"]
COPY ["src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.HttpUserContext/EndlessParties.Shared.Utils.HttpUserContext.csproj", "src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.HttpUserContext/"]
COPY ["src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.UserContext.Abstractions/EndlessParties.Shared.Utils.UserContext.Abstractions.csproj", "src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.UserContext.Abstractions/"]
COPY ["src/..Shared/Validations/EndlessParties.Shared.Validations/EndlessParties.Shared.Validations.csproj", "src/..Shared/Validations/EndlessParties.Shared.Validations/"]
COPY ["src/Events/.Shared/Database/EndlessParties.Events.Database/EndlessParties.Events.Database.csproj", "src/Events/.Shared/Database/EndlessParties.Events.Database/"]
COPY ["src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database.Migrations/EndlessParties.Shared.Utils.Database.Migrations.csproj", "src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database.Migrations/"]
COPY ["src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database/EndlessParties.Shared.Utils.Database.csproj", "src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database/"]
COPY ["src/Events/.Shared/Domain/EndlessParties.Events.Domain/EndlessParties.Events.Domain.csproj", "src/Events/.Shared/Domain/EndlessParties.Events.Domain/"]
COPY ["src/Events/Api/EndlessParties.Events.Api.App/EndlessParties.Events.Api.App.csproj", "src/Events/Api/EndlessParties.Events.Api.App/"]
COPY ["src/..Shared/Contracts/EndlessParties.Shared.Contracts.Events/EndlessParties.Shared.Contracts.Events.csproj", "src/..Shared/Contracts/EndlessParties.Shared.Contracts.Events/"]
COPY ["src/..Shared/EventBus/EndlessParties.Shared.EventBus.Abstractions/EndlessParties.Shared.EventBus.Abstractions.csproj", "src/..Shared/EventBus/EndlessParties.Shared.EventBus.Abstractions/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Extensions/EndlessParties.Shared.Exceptions.Extensions.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Extensions/"]
COPY ["src/..Shared/Utils/EndlessParties.Shared.Utils.DateTime/EndlessParties.Shared.Utils.DateTime.csproj", "src/..Shared/Utils/EndlessParties.Shared.Utils.DateTime/"]
COPY ["src/Events/.Shared/Repositories/EndlessParties.Events.Repositories.Abstractions/EndlessParties.Events.Repositories.Abstractions.csproj", "src/Events/.Shared/Repositories/EndlessParties.Events.Repositories.Abstractions/"]
COPY ["src/..Shared/Contracts/EndlessParties.Shared.Contracts/EndlessParties.Shared.Contracts.csproj", "src/..Shared/Contracts/EndlessParties.Shared.Contracts/"]
COPY ["src/Events/Api/EndlessParties.Events.Api.Infrastructure/EndlessParties.Events.Api.Infrastructure.csproj", "src/Events/Api/EndlessParties.Events.Api.Infrastructure/"]
COPY ["src/..Shared/EventBus/EndlessParties.Shared.EventBus.Kafka/EndlessParties.Shared.EventBus.Kafka.csproj", "src/..Shared/EventBus/EndlessParties.Shared.EventBus.Kafka/"]
COPY ["src/Events/.Shared/Repositories/EndlessParties.Events.Repositories/EndlessParties.Events.Repositories.csproj", "src/Events/.Shared/Repositories/EndlessParties.Events.Repositories/"]
RUN dotnet restore "./src/Events/Api/EndlessParties.Events.Api/EndlessParties.Events.Api.csproj"
COPY . .
WORKDIR "/src/src/Events/Api/EndlessParties.Events.Api"
RUN dotnet build "./EndlessParties.Events.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EndlessParties.Events.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EndlessParties.Events.Api.dll"]