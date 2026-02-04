# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /app/data && chown $APP_UID:$APP_UID /app/data # this is for sqlite support only; remove if not needed

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["source/Desic.Api/Desic.Api.csproj", "source/Desic.Api/"]
COPY ["source/Desic.Business/Desic.Business.csproj", "source/Desic.Business/"]
COPY ["source/Desic.EntityFrameworkCore/Desic.EntityFrameworkCore.csproj", "source/Desic.EntityFrameworkCore/"]
COPY ["source/Desic.EntityFrameworkCore.Sqlite/Desic.EntityFrameworkCore.Sqlite.csproj", "source/Desic.EntityFrameworkCore.Sqlite/"]
COPY ["source/Desic.EntityFrameworkCore.SqlServer/Desic.EntityFrameworkCore.SqlServer.csproj", "source/Desic.EntityFrameworkCore.SqlServer/"]
RUN dotnet restore "./source/Desic.Api/Desic.Api.csproj"
COPY . .
WORKDIR "/src/source/Desic.Api"
RUN dotnet build "./Desic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Desic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desic.Api.dll"]