FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN mkdir /home/app
WORKDIR /home/app

COPY src/ src/
COPY tests/ tests/

RUN dotnet restore "src/PixelService/PixelService.csproj"

RUN dotnet build "src/PixelService/PixelService.csproj" -c Release -o /build

FROM build AS publish
RUN dotnet publish "src/PixelService/PixelService.csproj" -c Release -o /publish /p:UseAppHost=false


# unit tests
FROM build AS unit-tests
RUN mkdir /tests_reports
CMD ["dotnet", "test", "--no-restore", "tests/unit/PixelServiceTests/PixelServiceTests.csproj", "--logger", "trx", "--results-directory", "/tests_reports"]


FROM base AS final
WORKDIR /home/app
COPY --from=publish /publish .
ENV ASPNETCORE_URLS=http://+:9999
EXPOSE 9999
ENTRYPOINT ["dotnet", "PixelService.dll"]
