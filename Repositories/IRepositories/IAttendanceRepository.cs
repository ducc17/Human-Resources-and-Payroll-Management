using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Attendance;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IAttendanceRepository
    {
        // Các hàm cơ bản
        Task<Attendance?> GetTodayAttendanceAsync(int employeeId, DateOnly date);
        Task<List<Attendance>> GetMyAttendanceHistoryAsync(int employeeId, DateOnly? fromDate, DateOnly? toDate, string? status);
        // Các hàm phục vụ Import Excel (IoT)
        Task<Employee?> GetEmployeeByCodeAsync(string employeeCode);
        Task AddMultipleAttendanceAsync(Attendance attendance);

        //lấy danh sách phòng ban
        Task<List<Department>> GetAllDepartmentsAsync();

        // hàm lọc
        Task<List<DailyAttendanceViewModel>> GetAllAttendancesAsync(string? search, DateOnly? fromDate, DateOnly? toDate, string? status, int? departmentId);
    }
}