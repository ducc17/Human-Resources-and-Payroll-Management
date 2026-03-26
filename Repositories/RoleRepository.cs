using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.ViewModels.Role;

namespace SmartHR_Payroll.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DBCodeFirstContext _context;
        public RoleRepository(DBCodeFirstContext context) { _context = context; }

        public async Task<List<RoleListViewModel>> GetAllAsync(string? sortBy = null)
        {
            var query = _context.Role.IgnoreQueryFilters()
                .Select(r => new RoleListViewModel
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    Description = r.Description,
                    IsDeleted = r.IsDeleted,
                    EmployeeCount = r.Employees.Count(),
                    CreatedBy = r.CreatedBy,
                    CreatedAt = r.CreatedAt
                });

            // Logic sắp xếp
            if (sortBy == "emp_desc")
                query = query.OrderByDescending(r => r.EmployeeCount).ThenBy(r => r.Name);
            else if (sortBy == "emp_asc")
                query = query.OrderBy(r => r.EmployeeCount).ThenBy(r => r.Name);
            else
                query = query.OrderBy(r => r.IsDeleted).ThenBy(r => r.Name);

            return await query.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id) => await _context.Role.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.RoleId == id);

        public async Task CreateAsync(Role role)
        {
            await _context.Role.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            var existingRole = await _context.Role.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.RoleId == role.RoleId);
            if (existingRole == null) return;

            existingRole.Name = role.Name;
            existingRole.Description = role.Description;
            existingRole.IsDeleted = role.IsDeleted;
            existingRole.UpdatedAt = role.UpdatedAt;
            existingRole.UpdatedBy = role.UpdatedBy;

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role != null)
            {
                role.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}