FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HistoryManagement/HistoryManagement.csproj", "HistoryManagement/"]
COPY ["History.Shared/History.Shared.csproj", "History.Shared/"]
RUN dotnet restore "HistoryManagement/HistoryManagement.csproj"
COPY . .
WORKDIR "/src/HistoryManagement"
RUN dotnet build "HistoryManagement.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "HistoryManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HistoryManagement.dll"]
