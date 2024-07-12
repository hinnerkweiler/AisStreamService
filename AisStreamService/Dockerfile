# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AisStreamService.csproj", "."]
RUN dotnet restore "./AisStreamService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AisStreamService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AisStreamService.csproj" -c Release -o /app/publish

# Use the base image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AisStreamService.dll"]
