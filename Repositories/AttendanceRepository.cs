using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Repositories.IRepositories;
namespace SmartHR_Payroll.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DBCodeFirstContext _context; // Thay bằng tên DbContext của bạn

        public AttendanceRepository(DBCodeFirstContext context) => _context = context;

        public async Task<Attendance?> GetTodayAttendanceAsync(int employeeId, DateOnly date) =>
            await _context.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date);

        public async Task AddAsync(Attendance att)
        {
            await _context.Attendances.AddAsync(att);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attendance att)
        {
            _context.Attendances.Update(att);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Attendance>> GetHistoryAsync(int employeeId) =>
            await _context.Attendances.Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.Date).ToListAsync();
    }
}
