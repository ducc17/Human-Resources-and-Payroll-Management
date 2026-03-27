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

                    // 1. FIX LỖI CRASH: Đếm tổng nhân viên của tất cả các "Công việc" (Job) thuộc Phòng ban này
                    EmployeeCount = d.Jobs.SelectMany(j => j.Employees).Count(),

                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt,

                    // 2. FIX LỖI HIỆU NĂNG: Dùng trực tiếp Navigation Property (Manager). 
                    // EF Core sẽ tự động biên dịch thành 1 câu lệnh LEFT JOIN duy nhất dưới SQL
                    ManagerCode = d.Manager != null ? d.Manager.EmployeeCode : null,
                    ManagerName = d.Manager != null ? (d.Manager.FirstName + " " + d.Manager.LastName) : null,
                    ManagerEmail = d.Manager != null ? d.Manager.Email : null
                });

            // Thêm logic sắp xếp (Giữ nguyên của bạn)
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
                .IgnoreQueryFilters() // Giữ nguyên để xuyên qua lớp bảo vệ "xóa mềm"
                .Where(d => d.DepartmentId == id)
                .Select(d => new DepartmentDetailViewModel
                {
                    DepartmentId = d.DepartmentId,
                    Code = d.Code,
                    Name = d.Name,

                    // 1. TỐI ƯU HIỆU NĂNG: Lấy tên Manager trực tiếp qua Khóa ngoại
                    ManagerName = d.Manager != null
                        ? (d.Manager.FirstName + " " + d.Manager.LastName)
                        : "Chưa bổ nhiệm",

                    // 2. CHUẨN HÓA MÔ HÌNH: Đi xuyên qua bảng Jobs để lấy tất cả Employees của phòng này
                    Employees = d.Jobs.SelectMany(j => j.Employees).Select(e => new EmployeeInDepartmentViewModel
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeCode = e.EmployeeCode,
                        FullName = e.FirstName + " " + e.LastName,
                        Email = e.Email ?? "Chưa cập nhật",
                        PhoneNumber = e.PhoneNumber ?? "Chưa cập nhật",

                        // 3. LẤY CHỨC VỤ: Đi ngược từ Nhân viên -> Job -> Position
                        PositionName = e.Job != null && e.Job.Position != null
                            ? e.Job.Position.Name
                            : "Chưa phân chức vụ"
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
            // BƯỚC 1: Tìm danh sách các Manager "đang bận" 
            // (Đang quản lý các phòng ban ĐANG HOẠT ĐỘNG và KHÁC với phòng ban hiện tại)
            var busyManagerIds = await _context.Departments
                .Where(d => !d.IsDeleted) // Chỉ xét những phòng ban chưa bị khóa
                .Where(d => d.ManagerId != null) // Có người quản lý
                .Where(d => d.ManagerId != currentManagerId) // Bỏ qua quản lý của phòng ban đang được Edit
                .Select(d => d.ManagerId.Value)
                .Distinct()
                .ToListAsync();

            // BƯỚC 2: Lọc danh sách Nhân viên thỏa mãn TẤT CẢ điều kiện
            return await _context.Employees
                .Include(e => e.Role) // Kết nối với bảng Role
                .Where(e => !e.IsDeleted) // Nhân viên chưa bị khóa/nghỉ việc
                .Where(e =>
                    (
                        // Điều kiện A: Có chức vụ là Quản lý/Manager
                        (e.Role != null && (e.Role.Name.Contains("Manage") || e.Role.Name.Contains("Quản lý")))

                        // Điều kiện B: Hoặc chính là Manager hiện tại (giữ lại họ để hiển thị lúc Edit)
                        || e.EmployeeId == currentManagerId
                    )
                )
                // ĐIỀU KIỆN QUYẾT ĐỊNH: Không nằm trong danh sách các Manager đang bận
                .Where(e => !busyManagerIds.Contains(e.EmployeeId))
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

        public async Task<bool> CheckManagerConflictAsync(int managerId, int currentDepartmentId)
        {
            // Kiểm tra xem có phòng ban nào ĐANG HOẠT ĐỘNG, khác với phòng hiện tại, mà ông này làm Manager không?
            return await _context.Departments
                .AnyAsync(d => !d.IsDeleted &&
                               d.ManagerId == managerId &&
                               d.DepartmentId != currentDepartmentId);
        }
    }
}