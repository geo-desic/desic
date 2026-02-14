# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["source/Desic.Core/Desic.Core.csproj", "source/Desic.Core/"]
COPY ["source/Desic.EntityFrameworkCore/Desic.EntityFrameworkCore.csproj", "source/Desic.EntityFrameworkCore/"]
COPY ["source/Desic.EntityFrameworkCore.SqlServer/Desic.EntityFrameworkCore.SqlServer.csproj", "source/Desic.EntityFrameworkCore.SqlServer/"]
COPY ["source/MediatR/MediatR.csproj", "source/MediatR/"]
COPY ["source/MediatR.Contracts/MediatR.Contracts.csproj", "source/MediatR.Contracts/"]
RUN dotnet restore "./source/Desic.EntityFrameworkCore.SqlServer/Desic.EntityFrameworkCore.SqlServer.csproj"
COPY . .
WORKDIR "/src/source/Desic.EntityFrameworkCore.SqlServer"
RUN dotnet build "./Desic.EntityFrameworkCore.SqlServer.csproj" -c $BUILD_CONFIGURATION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef
WORKDIR "/src/source/Desic.EntityFrameworkCore.SqlServer"
RUN dotnet ef migrations bundle \
    --output /src/efbundle \
    --self-contained \
    --verbose \
    --no-build \
    --configuration $BUILD_CONFIGURATION \
    --project Desic.EntityFrameworkCore.SqlServer.csproj \
    --context DesicContext

RUN chmod +x /src/efbundle

FROM base AS final
WORKDIR /app
COPY --from=publish /src/efbundle .
