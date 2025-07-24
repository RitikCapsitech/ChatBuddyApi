# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet publish "ChatbotFAQApi.csproj" -c Release -o /out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install -y libssl-dev
WORKDIR /app
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "ChatbotFAQApi.dll"] 