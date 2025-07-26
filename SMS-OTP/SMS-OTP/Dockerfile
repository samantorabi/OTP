FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SMS-OTP/SMS-OTP.csproj", "."]

RUN dotnet restore "./SMS-OTP.csproj"
COPY . .
#WORKDIR "/src"
#RUN dotnet build "./SMS-OTP.csproj" -c $BUILD_CONFIGURATION -o /build /p:UseAppHost=true

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SMS-OTP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SMS-OTP.dll"]
