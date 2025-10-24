namespace ApiMonetizationGateway.Services
{
    public interface IMonthlySummaryService
    {
        Task<List<MonthlySummary>> GetMonthlySummaries();

        Task<List<MonthlySummary>> GetMonthlySummariesByCustomer(string customerId);

    }
}
