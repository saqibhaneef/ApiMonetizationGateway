# ApiMonetizationGateway - Coding Challenge

Minimal .NET 8 sample implementing:
- Dynamic tier configuration
- Per-second rate limiting (memory cache)
- Monthly quota enforcement (SQLite + EF Core)
- Usage logging (ApiUsageLog)
- Monthly summarization background worker
- Unit/integration tests (xUnit)
- Dockerfile included

### Run locally (requires .NET 8 SDK)
dotnet run --project ApiMonetizationGateway

API is available at http://localhost:5000 (or port shown). Use header `X-Customer-Id` with seeded IDs:
- cust-free-1
- cust-pro-1

Example:
curl -H "X-Customer-Id: cust-free-1" http://localhost:5000/api/test/hello

### Tests
dotnet test tests/ApiMonetizationGateway.Tests

### Docker
docker build -t api-gateway ./ApiMonetizationGateway
docker run -p 8080:80 api-gateway