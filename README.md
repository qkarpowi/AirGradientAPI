# AirGradient API

A .NET 9 Web API service designed to collect and store air quality sensor data from AirGradient DIY monitoring devices. This service provides a robust endpoint for receiving environmental measurements including CO2 levels, PM2.5 particles, temperature, humidity, and WiFi signal strength.

## Features

- **RESTful API**: Clean HTTP endpoint for sensor data ingestion
- **Data Validation**: Comprehensive validation of sensor readings with appropriate ranges
- **PostgreSQL Integration**: Reliable data persistence with Entity Framework Core
- **Docker Support**: Containerized deployment for easy setup and scaling  
- **API Documentation**: Interactive Swagger/OpenAPI documentation
- **Logging**: Structured logging for monitoring and troubleshooting
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
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/get-started) (optional)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd AirGradientAPI
   ```

2. **Configure database connection**
   
   Update `appsettings.json` with your PostgreSQL connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=airgradient;Username=your_user;Password=your_password"
     }
   }
   ```

3. **Set up the database**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` with Swagger documentation at `https://localhost:5001/swagger`.

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

```
AirGradientAPI/
├── Controllers/           # API controllers
│   └── SensorDataController.cs
├── Entities/             # Database entities
│   └── SensorDatum.cs
├── Models/               # Data models and DTOs
│   ├── DataContext.cs
│   └── SensorDataModel.cs
├── Migrations/           # Entity Framework migrations
├── Properties/           # Launch settings
├── Dockerfile           # Docker configuration
└── Program.cs           # Application entry point
```

### Adding New Features

1. **Database changes**: Create migrations with `dotnet ef migrations add <name>`
2. **API endpoints**: Add new controllers following the existing pattern
3. **Validation**: Use data annotations in model classes
4. **Documentation**: Update Swagger attributes for new endpoints

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