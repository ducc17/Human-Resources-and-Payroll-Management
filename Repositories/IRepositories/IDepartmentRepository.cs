using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Department;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IDepartmentRepository
    {
        Task<List<DepartmentListViewModel>> GetAllWithDetailsAsync(string? sortBy = null);
        Task<DepartmentDetailViewModel?> GetDepartmentDetailAsync(int id);
        Task<Department?> GetByIdAsync(int id);
        Task<List<Employee>> GetEmployeesForDropdownAsync(int? currentManagerId = null); Task CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeactivateAsync(int id); // Hàm Soft Delete
    }
}