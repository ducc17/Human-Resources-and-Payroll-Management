using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Contract;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IContractService
    {
        Task<List<Employee>> GetEmployeesForCreateAsync();
        Task<(bool Success, string Message)> CreateContractAsync(CreateContractViewModel model, string actor);
    }
}
