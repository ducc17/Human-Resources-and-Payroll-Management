using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Contract;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IContractService
    {
        Task<List<Employee>> GetEmployeesForCreateAsync();
        Task<(bool Success, string Message)> CreateContractAsync(CreateContractViewModel model, string actor);
        Task<(bool Success, string Message)> CancelContractAsync(int contractId, string actor);
        Task<(bool Success, string Message)> ConfirmContractByEmployeeAsync(int contractId, int employeeId, string actor);
    }
}
