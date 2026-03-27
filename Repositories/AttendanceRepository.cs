using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DBCodeFirstContext _context;

        public AttendanceRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<Attendance?> GetTodayAttendanceAsync(int employeeId, DateOnly date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date);
        }

        public async Task<List<Attendance>> GetAllAttendancesAsync(string? search, DateOnly? fromDate, DateOnly? toDate, string? status, int? departmentId)
        {
            // Bắt đầu Query: Kéo theo dữ liệu Employee, Position và Department
            var query = _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Position)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .AsQueryable();

            // 1. Tìm kiếm theo Tên hoặc Mã NV
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
                query = query.Where(a =>
                    (a.Employee.EmployeeCode != null && a.Employee.EmployeeCode.ToLower().Contains(search)) ||
                    // Thay FullName bằng first_name và last_name (hoặc thuộc tính tương ứng trong Model Employee của bạn)
                    (a.Employee.FirstName != null && a.Employee.FirstName.ToLower().Contains(search)) ||
                    (a.Employee.LastName != null && a.Employee.LastName.ToLower().Contains(search))
                );
            }

            // 2. Lọc theo Từ ngày
            if (fromDate.HasValue)
            {
                query = query.Where(a => a.Date >= fromDate.Value);
            }

            // 3. Lọc theo Đến ngày
            if (toDate.HasValue)
            {
                query = query.Where(a => a.Date <= toDate.Value);
            }

            // 4. Lọc theo Trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "late") query = query.Where(a => a.IsLate);
                else if (status == "ontime") query = query.Where(a => !a.IsLate && a.CheckInTime != null);
                else if (status == "absent") query = query.Where(a => a.CheckInTime == null);
            }

            //5. Phòng ban
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                // Kiểm tra xem khóa ngoại trong Model Employee là DepartmentId hay department_id để sửa cho đúng nhé
                query = query.Where(a => a.Employee.DepartmentId == departmentId.Value);
            }

            return await query.OrderByDescending(a => a.Date).ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            // Giả định bảng Phòng ban của bạn tên là Departments
            return await _context.Departments.ToListAsync();
        }

        public async Task<List<Attendance>> GetMyAttendanceHistoryAsync(int employeeId, DateOnly? fromDate, DateOnly? toDate, string? status)
        {
            var query = _context.Attendances.Where(a => a.EmployeeId == employeeId).AsQueryable();

            // Lọc theo khoảng ngày
            if (fromDate.HasValue) query = query.Where(a => a.Date >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(a => a.Date <= toDate.Value);

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "late") query = query.Where(a => a.IsLate);
                else if (status == "ontime") query = query.Where(a => !a.IsLate && a.CheckInTime != null);
                else if (status == "absent") query = query.Where(a => a.CheckInTime == null);
            }

            return await query.OrderByDescending(a => a.Date).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByCodeAsync(string employeeCode)
        {
            // Giả định bảng Employee có trường employee_code
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
        }

        public async Task UpsertAttendanceAsync(Attendance attendance)
        {
            // Kiểm tra xem ngày hôm đó nhân viên đã có dữ liệu chưa
            var existingRecord = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == attendance.EmployeeId && a.Date == attendance.Date);

            if (existingRecord == null)
            {
                // Nếu chưa có -> Thêm mới
                await _context.Attendances.AddAsync(attendance);
            }
            else
            {
                // Nếu đã có -> Cập nhật lại giờ từ file Excel
                // Chỉ cập nhật nếu file Excel có dữ liệu, nếu không giữ nguyên giờ cũ
                existingRecord.CheckInTime = attendance.CheckInTime ?? existingRecord.CheckInTime;
                existingRecord.CheckOutTime = attendance.CheckOutTime ?? existingRecord.CheckOutTime;
                existingRecord.TotalHours = attendance.TotalHours;
                existingRecord.IsLate = attendance.IsLate;
                existingRecord.Note = attendance.Note;

                _context.Attendances.Update(existingRecord);
            }

            await _context.SaveChangesAsync();
        }
    }
}