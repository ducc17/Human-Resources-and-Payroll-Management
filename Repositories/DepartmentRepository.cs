using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.ViewModels.Department;

namespace SmartHR_Payroll.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DBCodeFirstContext _context;

        public DepartmentRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentListViewModel>> GetAllWithDetailsAsync(string? sortBy = null)
        {
            var query = _context.Departments.IgnoreQueryFilters()
                .Select(d => new DepartmentListViewModel
                {
                    DepartmentId = d.DepartmentId,
                    Code = d.Code,
                    Name = d.Name,
                    IsDeleted = d.IsDeleted,
                    EmployeeCount = d.Employees.Count(),
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt,
                    ManagerCode = _context.Employees.Where(e => e.EmployeeId == d.ManagerId).Select(e => e.EmployeeCode).FirstOrDefault(),
                    ManagerName = _context.Employees.Where(e => e.EmployeeId == d.ManagerId).Select(e => e.FirstName + " " + e.LastName).FirstOrDefault(),
                    ManagerEmail = _context.Employees.Where(e => e.EmployeeId == d.ManagerId).Select(e => e.Email).FirstOrDefault()
                });

            // Thêm logic sắp xếp
            if (sortBy == "emp_desc")
                query = query.OrderByDescending(d => d.EmployeeCount).ThenBy(d => d.Name);
            else if (sortBy == "emp_asc")
                query = query.OrderBy(d => d.EmployeeCount).ThenBy(d => d.Name);
            else
                query = query.OrderBy(d => d.IsDeleted).ThenBy(d => d.Name);

            return await query.ToListAsync();
        }

        public async Task<DepartmentDetailViewModel?> GetDepartmentDetailAsync(int id)
        {
            var detail = await _context.Departments
                .IgnoreQueryFilters() // THÊM DÒNG NÀY ĐỂ XUYÊN QUA LỚP BẢO VỆ "XÓA MỀM"
                .Where(d => d.DepartmentId == id)
                .Select(d => new DepartmentDetailViewModel
                {
                    DepartmentId = d.DepartmentId,
                    Code = d.Code,
                    Name = d.Name,
                    ManagerName = _context.Employees
                        .Where(e => e.EmployeeId == d.ManagerId)
                        .Select(e => e.FirstName + " " + e.LastName)
                        .FirstOrDefault() ?? "Chưa bổ nhiệm",

                    Employees = d.Employees.Select(e => new EmployeeInDepartmentViewModel
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeCode = e.EmployeeCode,
                        FullName = e.FirstName + " " + e.LastName,
                        Email = e.Email ?? "Chưa cập nhật",
                        PhoneNumber = e.PhoneNumber ?? "Chưa cập nhật",
                        PositionName = e.Position != null ? e.Position.Name : "Chưa phân chức vụ"
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return detail;
        }
        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<List<Employee>> GetEmployeesForDropdownAsync(int? currentManagerId = null)
        {
            return await _context.Employees
                .IgnoreQueryFilters()
                .Where(e => !e.IsDeleted && (e.RoleId == 3 || e.EmployeeId == currentManagerId))
                .ToListAsync();
        }
        public async Task CreateAsync(Department department)
        {
            await _context.Departments.AddAsync(department);

            // Nếu có chọn Manager, đổi Role của người đó thành Manager (Role ID = 2)
            if (department.ManagerId.HasValue)
            {
                var newManager = await _context.Employees.FindAsync(department.ManagerId.Value);
                if (newManager != null) newManager.RoleId = 2;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            // 1. Tìm phòng ban gốc trong CSDL
            // Thêm IgnoreQueryFilters() để xuyên thủng bộ lọc ẩn
            var existingDept = await _context.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.DepartmentId == department.DepartmentId);
            if (existingDept == null) return;

            // 2. Lưu lại ID của Manager cũ trước khi thay đổi
            int? oldManagerId = existingDept.ManagerId;

            // 3. Cập nhật thông tin từ Form truyền xuống (Bao gồm cả trạng thái IsDeleted)
            existingDept.Name = department.Name;
            existingDept.IsDeleted = department.IsDeleted; // Dòng này sẽ cập nhật trạng thái Khóa/Mở
            existingDept.UpdatedAt = department.UpdatedAt;
            existingDept.UpdatedBy = department.UpdatedBy;

            // 4. KIỂM TRA ĐỔI QUẢN LÝ
            if (oldManagerId != department.ManagerId)
            {
                // Quản lý cũ quay về làm Nhân viên (Role = 3)
                if (oldManagerId.HasValue)
                {
                    var oldManager = await _context.Employees.FindAsync(oldManagerId.Value);
                    if (oldManager != null) oldManager.RoleId = 3; // Nhớ check xem RoleId hay role_id nhé
                }

                // Quản lý mới lên chức Manager (Role = 2)
                if (department.ManagerId.HasValue)
                {
                    var newManager = await _context.Employees.FindAsync(department.ManagerId.Value);
                    if (newManager != null) newManager.RoleId = 2;
                }

                // Cập nhật Manager mới cho phòng ban
                existingDept.ManagerId = department.ManagerId;
            }

            // 5. Lưu toàn bộ thay đổi
            await _context.SaveChangesAsync();
        }
        public async Task DeactivateAsync(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept != null)
            {
                dept.IsDeleted = true;

                _context.Departments.Update(dept);
                await _context.SaveChangesAsync();
            }
        }
    }
}