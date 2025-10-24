namespace ApiMonetizationGateway.Services
{
    public interface IApiUsageLogsService
    {
        Task<List<ApiUsageLog>> GetUsageLogs();

        Task<List<ApiUsageLog>> GetUsageLogsByCustomer(string customerId);
    }

}
