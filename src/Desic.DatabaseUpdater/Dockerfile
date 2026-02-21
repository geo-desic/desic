# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Desic.DatabaseUpdater/Desic.DatabaseUpdater.csproj", "src/Desic.DatabaseUpdater/"]
COPY ["src/Desic.EntityFrameworkCore.Sqlite/Desic.EntityFrameworkCore.Sqlite.csproj", "src/Desic.EntityFrameworkCore.Sqlite/"]
COPY ["src/Desic.Data/Desic.Data.csproj", "src/Desic.Data/"]
COPY ["src/Desic.EntityFrameworkCore/Desic.EntityFrameworkCore.csproj", "src/Desic.EntityFrameworkCore/"]
COPY ["src/Desic/Desic.csproj", "src/Desic.Core/"]
COPY ["src/MediatR/MediatR.csproj", "src/MediatR/"]
COPY ["src/MediatR.Contracts/MediatR.Contracts.csproj", "src/MediatR.Contracts/"]
COPY ["src/Desic.EntityFrameworkCore.SqlServer/Desic.EntityFrameworkCore.SqlServer.csproj", "src/Desic.EntityFrameworkCore.SqlServer/"]
RUN dotnet restore "./src/Desic.DatabaseUpdater/Desic.DatabaseUpdater.csproj"
COPY . .
WORKDIR "/src/src/Desic.DatabaseUpdater"
RUN dotnet build "./Desic.DatabaseUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Desic.DatabaseUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desic.DatabaseUpdater.dll"]

CMD ["--provider", "SqlServer"]