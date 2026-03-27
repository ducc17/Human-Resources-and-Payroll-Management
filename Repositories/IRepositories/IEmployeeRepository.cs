using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeProfileAsync(int employeeId);
        Task<Employee?> GetEmployeeByIdAsync(int employeeId);
        Task<(List<Employee> Employees, int TotalCount)> GetEmployeesPagedAsync(int page, int pageSize);
        Task<List<Contract>> GetContractsByEmployeeIdAsync(int employeeId);
        Task<bool> SoftDeleteEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task<bool> EmployeeCodeExistsAsync(string employeeCode);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
        Task<bool> BankAccountNumberExistsAsync(string bankAccountNumber);
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<Position>> GetPositionsAsync();
        Task<List<Job>> GetJobsAsync();
        Task<List<Bank>> GetBanksAsync();
        Task<List<Role>> GetRolesAsync();
        Task AddEmployeeAsync(Employee employee);
        Task<Employee?> GetByIdAsync(int id);
    }
}