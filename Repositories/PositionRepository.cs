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

            // 2. FIX LỖI 1: Lọc xuyên qua bảng Job (Tìm chức vụ nào có ít nhất 1 Job thuộc Department này)
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                baseQuery = baseQuery.Where(p => p.Jobs.Any(j => j.DepartmentId == departmentId.Value));
            }

            // 3. Map sang ViewModel
            var query = baseQuery.Select(p => new PositionListViewModel
            {
                PositionId = p.PositionId,
                Code = p.Code,
                Name = p.Name,
                IsDeleted = p.IsDeleted,

                // FIX LỖI 3: Hiển thị tên phòng ban. 
                // Nếu có lọc theo phòng thì hiện tên phòng đó. Nếu không thì lấy đại phòng đầu tiên (hoặc báo chưa phân bổ).
                DepartmentName = departmentId.HasValue && departmentId.Value > 0
                    ? p.Jobs.FirstOrDefault(j => j.DepartmentId == departmentId.Value).Department.Name
                    : (p.Jobs.Any() ? p.Jobs.FirstOrDefault().Department.Name : "Chưa phân bổ"),

                // FIX LỖI 2: Đếm tổng số nhân viên đang giữ chức danh này thông qua Job
                EmployeeCount = p.Jobs.SelectMany(j => j.Employees).Count(),

                CreatedBy = p.CreatedBy,
                CreatedAt = p.CreatedAt
            });

            // 4. Sắp xếp (Dùng hàm OrderBy bình thường, EF Core sẽ tự dịch DepartmentName ở trên ra SQL)
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
            // Tìm chức vụ gốc trong Database
            var existingPos = await _context.Positions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.PositionId == position.PositionId);

            if (existingPos == null) return;

            // Chỉ cập nhật các thuộc tính bản thân của Position
            existingPos.Name = position.Name;
            existingPos.IsDeleted = position.IsDeleted;

            // Cập nhật Audit Log
            existingPos.UpdatedAt = position.UpdatedAt;
            existingPos.UpdatedBy = position.UpdatedBy;

            // LƯU Ý: Tuyệt đối KHÔNG đụng chạm gì đến DepartmentId ở đây.

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