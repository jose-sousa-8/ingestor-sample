FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN mkdir /home/app
WORKDIR /home/app

COPY src/ src/
COPY tests/ tests/

RUN dotnet restore "src/StorageService/StorageService.csproj"
RUN dotnet build "src/StorageService/StorageService.csproj" -c Release -o /build

FROM build AS publish
RUN dotnet publish "src/StorageService/StorageService.csproj" -c Release -o /publish /p:UseAppHost=false

# unit tests
FROM build AS unit-tests
RUN mkdir /tests_reports
CMD ["dotnet", "test", "--no-restore", "tests/unit/StorageServiceTests/StorageServiceTests.csproj", "--logger", "trx", "--results-directory", "/tests_reports"]


FROM base AS final
WORKDIR /home/app
COPY --from=publish /publish .
ENV ASPNETCORE_URLS=http://+:8888
EXPOSE 8888
ENTRYPOINT ["dotnet", "StorageService.dll"]
