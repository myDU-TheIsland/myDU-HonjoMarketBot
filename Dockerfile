FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["HonjoMarketBot.csproj", "./"]
RUN dotnet restore "HonjoMarketBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "HonjoMarketBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HonjoMarketBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apk add --no-cache \
      gcc \
      g++ \
      make \
      libc-dev \
      libstdc++ 

RUN apk add gcompat
RUN apk add musl-dev

CMD ["sh", "-c", "dotnet HonjoMarketBot.dll /config/dual.yaml /config/honjo.json"]