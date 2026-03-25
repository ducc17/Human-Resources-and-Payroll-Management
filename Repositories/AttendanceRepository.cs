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

        public async Task<List<Attendance>> GetMyHistoryAsync(int employeeId)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.Date) // Sắp xếp ngày mới nhất lên đầu
                .ToListAsync();
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