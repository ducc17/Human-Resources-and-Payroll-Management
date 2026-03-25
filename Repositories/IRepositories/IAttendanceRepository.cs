using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IAttendanceRepository
    {
        // Các hàm cơ bản
        Task<Attendance?> GetTodayAttendanceAsync(int employeeId, DateOnly date);
        Task<List<Attendance>> GetMyHistoryAsync(int employeeId);
        // Các hàm phục vụ Import Excel (IoT)
        Task<Employee?> GetEmployeeByCodeAsync(string employeeCode);
        Task UpsertAttendanceAsync(Attendance attendance);
    }
}