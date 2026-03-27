using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.ViewModels.Position;

namespace SmartHR_Payroll.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly DBCodeFirstContext _context;

        public PositionRepository(DBCodeFirstContext context) { _context = context; }

        public async Task<List<PositionListViewModel>> GetAllWithDetailsAsync(int? departmentId = null, string? sortBy = null)
        {
            // 1. Khởi tạo query gốc
            var baseQuery = _context.Positions.IgnoreQueryFilters().AsQueryable();

            // 2. Lọc theo Phòng ban (nếu có chọn)
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                baseQuery = baseQuery.Where(p => p.DepartmentId == departmentId.Value);
            }

            // 3. Map sang ViewModel
            var query = baseQuery.Select(p => new PositionListViewModel
            {
                PositionId = p.PositionId,
                Code = p.Code,
                Name = p.Name,
                IsDeleted = p.IsDeleted,
                DepartmentName = p.Department != null ? p.Department.Name : "Chưa xác định",
                EmployeeCount = p.Employees.Count(),
                CreatedBy = p.CreatedBy,
                CreatedAt = p.CreatedAt
            });

            // 4. Sắp xếp
            if (sortBy == "emp_desc")
                query = query.OrderByDescending(p => p.EmployeeCount).ThenBy(p => p.Name);
            else if (sortBy == "emp_asc")
                query = query.OrderBy(p => p.EmployeeCount).ThenBy(p => p.Name);
            else // Mặc định
                query = query.OrderBy(p => p.IsDeleted).ThenBy(p => p.DepartmentName).ThenBy(p => p.Name);

            return await query.ToListAsync();
        }

        public async Task<Position?> GetByIdAsync(int id)
        {
            return await _context.Positions.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PositionId == id);
        }

        public async Task<List<Department>> GetDepartmentsForDropdownAsync()
        {
            // Chỉ lấy các phòng ban đang hoạt động
            return await _context.Departments.IgnoreQueryFilters().Where(d => !d.IsDeleted).ToListAsync();
        }

        public async Task CreateAsync(Position position)
        {
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Position position)
        {
            var existingPos = await _context.Positions.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PositionId == position.PositionId);
            if (existingPos == null) return;

            existingPos.Name = position.Name;
            existingPos.DepartmentId = position.DepartmentId;
            existingPos.IsDeleted = position.IsDeleted;
            existingPos.UpdatedAt = position.UpdatedAt;
            existingPos.UpdatedBy = position.UpdatedBy;

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var pos = await _context.Positions.FindAsync(id);
            if (pos != null)
            {
                pos.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}