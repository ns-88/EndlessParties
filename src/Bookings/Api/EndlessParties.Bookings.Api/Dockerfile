FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Bookings/Api/EndlessParties.Bookings.Api/EndlessParties.Bookings.Api.csproj", "src/Bookings/Api/EndlessParties.Bookings.Api/"]
COPY ["src/..Shared/Contracts/EndlessParties.Shared.Contracts.Events/EndlessParties.Shared.Contracts.Events.csproj", "src/..Shared/Contracts/EndlessParties.Shared.Contracts.Events/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions/EndlessParties.Shared.Exceptions.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Models/EndlessParties.Shared.Exceptions.Models.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Models/"]
COPY ["src/..Shared/Utils/EndlessParties.Shared.Utils.WebApiExtensions/EndlessParties.Shared.Utils.WebApiExtensions.csproj", "src/..Shared/Utils/EndlessParties.Shared.Utils.WebApiExtensions/"]
COPY ["src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.HttpUserContext/EndlessParties.Shared.Utils.HttpUserContext.csproj", "src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.HttpUserContext/"]
COPY ["src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.UserContext.Abstractions/EndlessParties.Shared.Utils.UserContext.Abstractions.csproj", "src/..Shared/Utils/UserContext/EndlessParties.Shared.Utils.UserContext.Abstractions/"]
COPY ["src/..Shared/Validations/EndlessParties.Shared.Validations/EndlessParties.Shared.Validations.csproj", "src/..Shared/Validations/EndlessParties.Shared.Validations/"]
COPY ["src/Bookings/Api/EndlessParties.Bookings.Api.App/EndlessParties.Bookings.Api.App.csproj", "src/Bookings/Api/EndlessParties.Bookings.Api.App/"]
COPY ["src/..Shared/Contracts/EndlessParties.Shared.Contracts.Bookings/EndlessParties.Shared.Contracts.Bookings.csproj", "src/..Shared/Contracts/EndlessParties.Shared.Contracts.Bookings/"]
COPY ["src/..Shared/EventBus/EndlessParties.Shared.EventBus.Abstractions/EndlessParties.Shared.EventBus.Abstractions.csproj", "src/..Shared/EventBus/EndlessParties.Shared.EventBus.Abstractions/"]
COPY ["src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Extensions/EndlessParties.Shared.Exceptions.Extensions.csproj", "src/..Shared/Exceptions/EndlessParties.Shared.Exceptions.Extensions/"]
COPY ["src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database/EndlessParties.Shared.Utils.Database.csproj", "src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database/"]
COPY ["src/..Shared/Utils/EndlessParties.Shared.Utils.DateTime/EndlessParties.Shared.Utils.DateTime.csproj", "src/..Shared/Utils/EndlessParties.Shared.Utils.DateTime/"]
COPY ["src/Bookings/.Shared/Domain/EndlessParties.Bookings.Domain/EndlessParties.Bookings.Domain.csproj", "src/Bookings/.Shared/Domain/EndlessParties.Bookings.Domain/"]
COPY ["src/Bookings/.Shared/Repositories/EndlessParties.Bookings.Repositories.Abstractions/EndlessParties.Bookings.Repositories.Abstractions.csproj", "src/Bookings/.Shared/Repositories/EndlessParties.Bookings.Repositories.Abstractions/"]
COPY ["src/Bookings/Api/EndlessParties.Bookings.Api.Infrastructure/EndlessParties.Bookings.Api.Infrastructure.csproj", "src/Bookings/Api/EndlessParties.Bookings.Api.Infrastructure/"]
COPY ["src/..Shared/EventBus/EndlessParties.Shared.EventBus.Kafka/EndlessParties.Shared.EventBus.Kafka.csproj", "src/..Shared/EventBus/EndlessParties.Shared.EventBus.Kafka/"]
COPY ["src/Bookings/.Shared/Repositories/EndlessParties.Bookings.Repositories/EndlessParties.Bookings.Repositories.csproj", "src/Bookings/.Shared/Repositories/EndlessParties.Bookings.Repositories/"]
COPY ["src/Bookings/.Shared/Database/EndlessParties.Bookings.Database/EndlessParties.Bookings.Database.csproj", "src/Bookings/.Shared/Database/EndlessParties.Bookings.Database/"]
COPY ["src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database.Migrations/EndlessParties.Shared.Utils.Database.Migrations.csproj", "src/..Shared/Utils/Database/EndlessParties.Shared.Utils.Database.Migrations/"]
RUN dotnet restore "./src/Bookings/Api/EndlessParties.Bookings.Api/EndlessParties.Bookings.Api.csproj"
COPY . .
WORKDIR "/src/src/Bookings/Api/EndlessParties.Bookings.Api"
RUN dotnet build "./EndlessParties.Bookings.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EndlessParties.Bookings.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EndlessParties.Bookings.Api.dll"]