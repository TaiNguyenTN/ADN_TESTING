FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj (đúng theo folder thực của bạn)
COPY ["APIGateway.csproj", "."]

RUN dotnet restore "APIGateway.csproj"

# Copy toàn bộ source
COPY . .

WORKDIR "/src"
RUN dotnet build "APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy ocelot.json (vì nó nằm CÙNG folder với Dockerfile)
COPY ocelot.json ./ocelot.json

ENTRYPOINT ["dotnet", "APIGateway.dll"]
