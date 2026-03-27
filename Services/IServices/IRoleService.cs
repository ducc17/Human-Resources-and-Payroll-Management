using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Role;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IRoleService
    {
        Task<List<RoleListViewModel>> GetAllAsync(string? sortBy = null);
        Task<Role?> GetByIdAsync(int id);
        Task CreateAsync(Role role, string currentUserName);
        Task UpdateAsync(Role role, string currentUserName);
        Task DeactivateAsync(int id);
    }
}