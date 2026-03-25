using global::SmartHR_Payroll.Models;
using SmartHR_Payroll.Models;
namespace SmartHR_Payroll.Services.IServices
{


    namespace SmartHR_Payroll.Services.IServices
    {
        public interface IAttendanceService
        {
            Task<Attendance?> GetTodayAttendanceAsync(int employeeId);
            Task<List<Attendance>> GetHistoryAsync(int employeeId);
            Task<(bool Success, string Message)> CheckInAsync(int employeeId);
            Task<(bool Success, string Message)> CheckOutAsync(int employeeId);
        }
    }
}
