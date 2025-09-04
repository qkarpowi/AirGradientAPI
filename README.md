# AirGradient API

[![Build and Test](https://github.com/qkarpowi/AirGradientAPI/actions/workflows/build.yml/badge.svg)](https://github.com/qkarpowi/AirGradientAPI/actions/workflows/build.yml)
[![Deploy](https://github.com/qkarpowi/AirGradientAPI/actions/workflows/deploy.yml/badge.svg)](https://github.com/qkarpowi/AirGradientAPI/actions/workflows/deploy.yml)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A .NET 9 Web API service designed to collect and store air quality sensor data from AirGradient DIY monitoring devices. This service provides a robust endpoint for receiving environmental measurements including CO2 levels, PM2.5 particles, temperature, humidity, and WiFi signal strength. The solution includes a .NET Aspire project to quickly run and test.

## Features

- **RESTful API**: Clean HTTP endpoint for sensor data ingestion
- **Data Validation**: Comprehensive validation of sensor readings with appropriate ranges
- **PostgreSQL Integration**: Reliable data persistence with Entity Framework Core
- **.NET Aspire**: Modern cloud-native orchestration for development and deployment
- **Docker Support**: Containerized deployment for easy setup
- **API Documentation**: Interactive Swagger/OpenAPI documentation
- **Unit Testing**: Comprehensive test coverage with xUnit
- **CI/CD**: Automated build and deployment with GitHub Actions
- **Logging**: Structured logging for monitoring and troubleshooting with OpenTelemetry
- **Database Indexing**: Optimized database performance for time-series data queries

## Supported Sensor Data

The API accepts the following environmental measurements:

| Metric | Description | Range | Unit |
|--------|-------------|--------|------|
| **WiFi** | Signal strength | -100 to 0 | dBm |
| **CO2** | Carbon dioxide concentration | 0 to 50,000 | ppm |
| **PM2.5** | Fine particulate matter | 0 to 1,000 | µg/m³ |
| **Temperature** | Ambient temperature | -40 to 176 | °F |
| **Humidity** | Relative humidity | 0 to 100 | % |

## Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) (for Aspire orchestration)
- [PostgreSQL 12+](https://www.postgresql.org/download/) (if running without Aspire)

### Local Development with Aspire

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd AirGradientAPI
   ```

2. **Run with .NET Aspire (Recommended)**
   ```bash
   dotnet run --project AirGradientAPI.AppHost
   ```
   
   This will start the Aspire dashboard where you can monitor all services. The API will be automatically configured with a PostgreSQL container.

3. **Manual setup (Alternative)**
   
   Configure database connection in `AirGradientAPI/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=airgradient;Username=your_user;Password=your_password"
     }
   }
   ```

   Set up the database:
   ```bash
   dotnet ef database update --project AirGradientAPI
   ```

   Run the API directly:
   ```bash
   dotnet run --project AirGradientAPI
   ```

The API will be available at `https://localhost:8080` with Swagger documentation at `https://localhost:8080/swagger`.

### Running Tests

```bash
dotnet test AirGradientAPI.Tests
```

### Docker Deployment

1. **Build the container**
   ```bash
   docker build -t airgradient-api .
   ```

2. **Run with environment variables**
   ```bash
   docker run -p 5000:8080 \
     -e ConnectionStrings__DefaultConnection="Host=your-db-host;Database=airgradient;Username=user;Password=pass" \
     airgradient-api
   ```

## API Usage

### Send Sensor Data

**Endpoint:** `POST /api/v1/sensors/airgradient:{chipId}/measures`

**Example Request:**
```bash
curl -X POST "https://your-api-host/api/v1/sensors/airgradient:ABC123/measures" \
  -H "Content-Type: application/json" \
  -d '{
    "wifi": -45,
    "rco2": 420,
    "pm02": 12,
    "atmp": 72.5,
    "rhum": 45
  }'
```

**Example Response:**
```json
{
  "message": "Sensor data received successfully."
}
```

### Error Responses

The API provides detailed error messages for validation failures:

```json
{
  "error": "Validation failed",
  "details": {
    "Rco2": ["CO2 reading must be between 0 and 50000 ppm"],
    "Wifi": ["WiFi signal strength must be between -100 and 0 dBm"]
  }
}
```

## Database Schema

The sensor data is stored in the `SensorData` table with the following structure:

```sql
CREATE TABLE "SensorData" (
    "Id" bigserial PRIMARY KEY,
    "ChipId" varchar(50) NOT NULL,
    "Wifi" integer NOT NULL,
    "Rco2" integer NOT NULL,
    "Pm02" integer NOT NULL,
    "Atmp" real NOT NULL,
    "Rhum" integer NOT NULL,
    "Timestamp" timestamp with time zone
);

-- Performance indexes
CREATE INDEX "IX_SensorData_ChipId" ON "SensorData" ("ChipId");
CREATE INDEX "IX_SensorData_Timestamp" ON "SensorData" ("Timestamp");
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | Required |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Production |
| `ASPNETCORE_URLS` | Binding URLs | `http://+:8080` |

### Application Settings

Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## AirGradient Device Configuration

To configure your AirGradient device to send data to this API:

1. **Set the API endpoint** in your device configuration:
   ```
   http://your-api-host/api/v1/sensors/airgradient:{your-device-chip-id}/measures
   ```

2. **Ensure your device sends data** in the expected JSON format with the required fields.

3. **Use a unique chip ID** for each device to differentiate between multiple sensors.

## Monitoring and Maintenance

### Health Checks

The API logs all operations and errors. Monitor the application logs for:
- Successful data ingestion: `"Successfully saved sensor data for chipId: {ChipId}"`
- Database errors: `"Database update error while saving sensor data"`
- Validation failures: Detailed error responses in API calls

### Database Maintenance

- **Backup**: Regular PostgreSQL backups recommended
- **Cleanup**: Consider implementing data retention policies for long-term storage
- **Monitoring**: Watch database size and query performance

## Development

### Project Structure

The solution now uses .NET Aspire for orchestration and includes multiple projects:

```
AirGradientAPI/
├── AirGradientAPI/                    # Main API project
│   ├── Controllers/                   # API controllers
│   │   └── SensorDataController.cs
│   ├── Entities/                      # Database entities
│   │   └── SensorDatum.cs
│   ├── Models/                        # Data models and DTOs
│   │   ├── DataContext.cs
│   │   └── SensorDataModel.cs
│   ├── Migrations/                    # Entity Framework migrations
│   ├── Properties/                    # Launch settings
│   ├── Extensions.cs                  # Service extensions
│   ├── Dockerfile                     # Docker configuration
│   ├── Program.cs                     # API application entry point
│   └── appsettings.json              # API configuration
├── AirGradientAPI.AppHost/           # Aspire orchestration host
│   ├── Properties/                    # Launch settings
│   ├── Program.cs                     # Aspire host entry point
│   └── appsettings.json              # Aspire configuration
├── AirGradientAPI.Tests/             # Unit and integration tests
│   ├── Controller/                    # Controller tests
│   │   └── SensorControllerTests.cs
│   └── Models/                        # Model tests
├── .github/workflows/                # GitHub Actions
│   ├── build.yml                     # Build workflow
│   └── deploy.yml                    # Deployment workflow
└── AirGradientAPI.sln               # Solution file
```

### Adding New Features

1. **Database changes**: Create migrations with `dotnet ef migrations add <name> --project AirGradientAPI`
2. **API endpoints**: Add new controllers following the existing pattern in `AirGradientAPI/Controllers`
3. **Validation**: Use data annotations in model classes
4. **Tests**: Add corresponding tests in `AirGradientAPI.Tests`
5. **Documentation**: Update Swagger attributes for new endpoints

### Development Workflow

1. **Start Aspire orchestration**: `dotnet run --project AirGradientAPI.AppHost`
2. **Make changes**: Edit files in the `AirGradientAPI` project
3. **Run tests**: `dotnet test`
4. **View logs**: Use the Aspire dashboard for real-time monitoring

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Update documentation
6. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
- Check the application logs for error details
- Review the Swagger documentation at `/swagger`
- Verify database connectivity and migration status
- Ensure AirGradient devices are sending properly formatted data

---

**Version**: 1.0  
**Framework**: .NET 9.0  
**Database**: PostgreSQL  
**API Version**: v1.0