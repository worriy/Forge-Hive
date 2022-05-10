FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Hive.csproj .
RUN dotnet restore Hive.csproj
COPY . .
WORKDIR /src
RUN dotnet build Hive.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Hive.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hive.Backend.dll"]