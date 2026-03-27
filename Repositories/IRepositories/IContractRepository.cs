using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IContractRepository
    {
        Task<List<Employee>> GetEmployeesForContractAsync();
        Task<bool> EmployeeExistsAsync(int employeeId);
        Task<bool> ContractNumberExistsAsync(string contractNumber);
        Task AddContractAsync(Contract contract);
    }
}
