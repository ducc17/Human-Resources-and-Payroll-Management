using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Profile;
using SmartHR_Payroll.ViewModels.Employee;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IEmployeeService
    {
        Task<ProfileViewModel?> GetProfileAsync(int employeeId);
        Task<EditProfileViewModel?> GetEditProfileAsync(int employeeId);
        Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileViewModel model);
        Task<(List<Job> Jobs, List<Bank> Banks, List<Role> Roles)> GetCreateEmployeeLookupsAsync();
        Task<(bool Success, string Message)> CreateEmployeeAsync(Employee employee, string actor);
        Task<EmployeeListViewModel> GetEmployeesPagedAsync(
             int page,
             int pageSize,
             int? departmentId,
             string keyword,
             string status
         );
        Task<(bool Success, string Message)> BanEmployeeAsync(int employeeId, string actor);
        Task<(bool Success, string Message)> UnbanEmployeeAsync(int employeeId, string actor);
        Task<EmployeeContractsViewModel?> GetEmployeeContractsAsync(int employeeId);
        Task<Employee?> GetByIdAsync(int id);
    }
}