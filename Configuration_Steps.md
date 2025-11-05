# Configuration Steps

## Connection Strings
Update in appsettings.json:
```json
"ConnectionStrings": {
  "FTA": "Server=HABIBA\\SQLEXPRESS;Database=Electricity_FTA;Integrated Security=True;TrustServerCertificate=True;"
}
```

## API Configuration
- Port: 5032
- Endpoints:
  - POST /api/CuttingDownA/generate-test-data?count=10
  - POST /api/CuttingDownB/generate-test-data?count=10
  - GET /api/CuttingDownA/health

## Console App Configuration
- Cycle Interval: 10 seconds
- Sources: A (Cabins) & B (Cables)
- Database: Electricity_FTA

## Portal Configuration
- Port: 5000
- Auto-refresh: Real-time data
- Database: Electricity_FTA