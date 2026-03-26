using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Role;

namespace SmartHR_Payroll.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository) { _roleRepository = roleRepository; }

        public async Task<List<RoleListViewModel>> GetAllAsync(string? sortBy = null) => await _roleRepository.GetAllAsync(sortBy);
        public async Task<Role?> GetByIdAsync(int id) => await _roleRepository.GetByIdAsync(id);

        public async Task CreateAsync(Role role, string currentUserName)
        {
            role.IsDeleted = false;
            role.CreatedBy = currentUserName;
            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedBy = currentUserName;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.CreateAsync(role);
        }

        public async Task UpdateAsync(Role role, string currentUserName)
        {
            role.UpdatedBy = currentUserName;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeactivateAsync(int id) => await _roleRepository.DeactivateAsync(id);
    }
}