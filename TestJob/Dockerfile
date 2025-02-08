#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TestJob/TestJob.csproj", "TestJob/"]
RUN dotnet restore "TestJob/TestJob.csproj"
COPY . .
WORKDIR "/src/TestJob"
RUN dotnet build "TestJob.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestJob.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestJob.dll"]