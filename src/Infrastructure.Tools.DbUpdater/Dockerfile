# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Infrastructure.Tools.DbUpdater/Infrastructure.Tools.DbUpdater.csproj", "src/Infrastructure.Tools.DbUpdater/"]
COPY ["src/Infrastructure.Data.Providers/Infrastructure.Data.Providers.csproj", "src/Infrastructure.Data.Providers/"]
COPY ["src/Infrastructure.Data.Sqlite/Infrastructure.Data.Sqlite.csproj", "src/Infrastructure.Data.Sqlite/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/Infrastructure.Data.SqlServer/Infrastructure.Data.SqlServer.csproj", "src/Infrastructure.Data.SqlServer/"]
RUN dotnet restore "./src/Infrastructure.Tools.DbUpdater/Infrastructure.Tools.DbUpdater.csproj"
COPY . .
WORKDIR "/src/src/Infrastructure.Tools.DbUpdater"
RUN dotnet build "./Infrastructure.Tools.DbUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Infrastructure.Tools.DbUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desic.Infrastructure.Tools.DbUpdater.dll"]
CMD ["--provider", "SqlServer"]