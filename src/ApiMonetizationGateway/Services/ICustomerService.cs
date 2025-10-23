namespace ApiMonetizationGateway.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetCustomers();
    }
}
