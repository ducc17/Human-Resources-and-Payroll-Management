using Microsoft.AspNetCore.Http;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Attendance;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IAttendanceService
    {
        // Hàm Import Excel trả về: Số thành công, Số lỗi, Danh sách chi tiết lỗi
        Task<(int SuccessCount, int ErrorCount, List<string> ErrorMessages)> ImportExcelAsync(IFormFile file);

        Task<List<Attendance>> GetMyAttendanceHistoryAsync(int employeeId);
    }
}