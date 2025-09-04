#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /source
COPY *.csproj .
RUN dotnet restore --disable-parallel
COPY . .
RUN dotnet publish -c release -o /app --no-restore


#Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "AirGradientAPI.dll"]