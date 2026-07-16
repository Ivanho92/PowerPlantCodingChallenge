FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8888
EXPOSE 8888

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PowerPlantCodingChallenge/PowerPlantCodingChallenge.csproj", "PowerPlantCodingChallenge/"]
RUN dotnet restore "PowerPlantCodingChallenge/PowerPlantCodingChallenge.csproj" --verbosity detailed
COPY . .
WORKDIR "/src/PowerPlantCodingChallenge"
RUN dotnet build "PowerPlantCodingChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PowerPlantCodingChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PowerPlantCodingChallenge.dll"]