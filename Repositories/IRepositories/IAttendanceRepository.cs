using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetTodayAttendanceAsync(int employeeId, DateOnly date);
        Task AddAsync(Attendance att);
        Task UpdateAsync(Attendance att);
        Task<List<Attendance>> GetHistoryAsync(int employeeId);
    }
}
