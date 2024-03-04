#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Desic.Api/Desic.Api.csproj", "Desic.Api/"]
RUN dotnet restore "Desic.Api/Desic.Api.csproj"
COPY . .
WORKDIR "/src/Desic.Api"
RUN dotnet build "Desic.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Desic.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desic.Api.dll"]