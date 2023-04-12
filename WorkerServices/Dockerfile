#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WorkerServices/WorkerServices.csproj", "WorkerServices/"]
COPY ["RepositoryLayer/RepositoryLayer.csproj", "RepositoryLayer/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["Profiling/Profiling.csproj", "Profiling/"]
COPY ["SharedLib/SharedLib.csproj", "SharedLib/"]
RUN dotnet restore "WorkerServices/WorkerServices.csproj"
COPY . .
WORKDIR "/src/WorkerServices"
RUN dotnet build "WorkerServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerServices.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerServices.dll"]