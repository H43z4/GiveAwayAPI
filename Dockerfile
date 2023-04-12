#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APIGateway/APIGateway.csproj", "APIGateway/"]
COPY ["Authentication/Authentication.csproj", "Authentication/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["RepositoryLayer/RepositoryLayer.csproj", "RepositoryLayer/"]
COPY ["SharedLib/SharedLib.csproj", "SharedLib/"]
COPY ["UserManagement/UserManagement.csproj", "UserManagement/"]
COPY ["Profiling/Profiling.csproj", "Profiling/"]
COPY ["Biometric/Biometric.csproj", "Biometric/"]
COPY ["AMQPSvc/AMQPSvc.csproj", "AMQPSvc/"]
COPY ["Setup/Setup.csproj", "Setup/"]
COPY ["SeriesNumberPool/SeriesNumberPool.csproj", "SeriesNumberPool/"]
COPY ["VehicleRegistration/VehicleRegistration.csproj", "VehicleRegistration/"]
COPY ["Logging/Logging.csproj", "Logging/"]
COPY ["Admin/Admin.csproj", "Admin/"]
RUN dotnet restore "APIGateway/APIGateway.csproj"
COPY . .
WORKDIR "/src/APIGateway"
RUN dotnet build "APIGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"]
