using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeProfileAsync(int employeeId);
        Task UpdateEmployeeAsync(Employee employee);
        Task<Employee?> GetByIdAsync(int id);
    }
}