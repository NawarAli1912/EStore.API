# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first to leverage Docker layer caching on restore
COPY EStore.API.sln ./
COPY src/Domain/Domain.csproj                 src/Domain/
COPY src/SharedKernel/SharedKernel.csproj     src/SharedKernel/
COPY src/Contracts/Contracts.csproj           src/Contracts/
COPY src/Application/Application.csproj        src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Presentation/Presentation.csproj     src/Presentation/
RUN dotnet restore src/Presentation/Presentation.csproj

# Copy the remaining source and publish
COPY src/ src/
RUN dotnet publish src/Presentation/Presentation.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "Presentation.dll"]
