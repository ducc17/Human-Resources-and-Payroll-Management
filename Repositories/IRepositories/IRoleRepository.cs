using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Role;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IRoleRepository
    {
        Task<List<RoleListViewModel>> GetAllAsync(string? sortBy = null);
        Task<Role?> GetByIdAsync(int id);
        Task CreateAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeactivateAsync(int id);
    }
}