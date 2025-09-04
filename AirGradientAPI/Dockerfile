#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /source
COPY . .
RUN dotnet restore --disable-parallel
RUN dotnet publish -c Release -o /app


#Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "AirGradientAPI.dll"]