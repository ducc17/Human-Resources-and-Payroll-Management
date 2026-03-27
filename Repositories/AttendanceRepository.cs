using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.ViewModels.Attendance;

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

        // Trong file AttendanceRepository.cs
        public async Task<List<DailyAttendanceViewModel>> GetAllAttendancesAsync(string? search, DateOnly? fromDate, DateOnly? toDate, string? status, int? departmentId)
        {
            // 1. Lấy tất cả dữ liệu gốc ra trước (đã áp dụng bộ lọc search, date...)
            var query = _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Job) // Bao gồm Job để lấy phòng ban
                .ThenInclude(j => j.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Employee.EmployeeCode.Contains(search)
                                      || a.Employee.FirstName.Contains(search)
                                      || a.Employee.LastName.Contains(search));
            }

            if (fromDate.HasValue) query = query.Where(a => a.Date >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(a => a.Date <= toDate.Value);

            // Lọc theo phòng ban (thông qua Job)
            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(a => a.Employee.Job != null && a.Employee.Job.DepartmentId == departmentId.Value);
            }

            // Tải toàn bộ danh sách thô về RAM để xử lý Gom nhóm
            var rawData = await query.ToListAsync();

            // 2. LOGIC GOM NHÓM (GROUP BY)
            // Nhóm theo Mã nhân viên VÀ Ngày
            var groupedData = rawData
                .GroupBy(a => new { a.EmployeeId, a.Date })
                .Select(group =>
                {
                    var firstRecord = group.First();
                    var firstIn = group.Where(x => x.CheckInTime.HasValue).Min(x => x.CheckInTime);
                    var lastOut = group.Where(x => x.CheckOutTime.HasValue).Max(x => x.CheckOutTime);

                    // Tính lại tổng giờ làm việc trong ngày
                    decimal totalHrs = 0;
                    if (firstIn.HasValue && lastOut.HasValue)
                    {
                        totalHrs = (decimal)(lastOut.Value - firstIn.Value).TotalHours;
                        totalHrs = Math.Round(totalHrs, 2);
                    }

                    return new DailyAttendanceViewModel
                    {
                        EmployeeId = group.Key.EmployeeId,
                        EmployeeCode = firstRecord.Employee.EmployeeCode,
                        FullName = firstRecord.Employee.FirstName + " " + firstRecord.Employee.LastName,
                        DepartmentName = firstRecord.Employee.Job?.Department?.Name ?? "Chưa xếp phòng",
                        Date = group.Key.Date,

                        FirstCheckIn = firstIn,
                        LastCheckOut = lastOut,
                        TotalHours = totalHrs,

                        // Quy định 8:30 là muộn (bạn có thể thay đổi số này)
                        IsLate = firstIn.HasValue && firstIn.Value > new TimeSpan(8, 30, 0),

                        // Đổ toàn bộ các lần quẹt thẻ trong ngày vào danh sách này
                        // Sắp xếp theo giờ để hiển thị cho đẹp
                        Details = group.OrderBy(x => x.CheckInTime ?? x.CheckOutTime).ToList()
                    };
                })
                .OrderByDescending(x => x.Date)
                .ThenBy(x => x.EmployeeCode)
                .ToList();

            // 3. Lọc theo trạng thái (Hợp lệ / Không hợp lệ)
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "HopLe")
                    groupedData = groupedData.Where(a => a.IsPerfect).ToList();
                else if (status == "KhongHopLe")
                    groupedData = groupedData.Where(a => !a.IsPerfect).ToList();
            }

            return groupedData;
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

        public async Task AddMultipleAttendanceAsync(Attendance attendance)
        {
            // BƯỚC 1: KIỂM TRA TRÙNG LẶP TUYỆT ĐỐI (SOFT DUPLICATE CHECK)
            // Để tránh lỗi khi HR ấn Import 2 lần cái file Excel y hệt nhau.
            // Chúng ta chỉ Insert nếu dòng này chưa tồn tại chính xác đến từng giây quẹt.

            bool isExisting = await _context.Attendances.AnyAsync(a =>
                a.EmployeeId == attendance.EmployeeId &&
                a.Date == attendance.Date &&
                a.CheckInTime == attendance.CheckInTime &&
                a.CheckOutTime == attendance.CheckOutTime);

            if (!isExisting)
            {
                // BƯỚC 2: NẾU CA NÀY CHƯA CÓ -> THÊM MỚI (INSERT)
                // Cấu trúc DB của bạn dùng attendance_id tự tăng nên ko lo trùng khóa chính.
                await _context.Attendances.AddAsync(attendance);
                await _context.SaveChangesAsync();
            }
        }
    }
}