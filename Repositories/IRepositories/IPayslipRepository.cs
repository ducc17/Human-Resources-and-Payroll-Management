using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IPayslipRepository
    {
        Task<Employee?> GetEmployeeByIdAsync(int employeeId);
        Task<List<Payslip>> GetMyPayslipsAsync(int employeeId);
        Task<Payslip?> GetPayslipDetailAsync(int payslipId);
    }
}
