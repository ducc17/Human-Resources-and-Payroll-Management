using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.Services.IServices.SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Attendance?> GetTodayAttendanceAsync(int employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return await _attendanceRepository.GetTodayAttendanceAsync(employeeId, today);
        }

        public async Task<List<Attendance>> GetHistoryAsync(int employeeId)
        {
            return await _attendanceRepository.GetHistoryAsync(employeeId);
        }

        public async Task<(bool Success, string Message)> CheckInAsync(int employeeId)
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            // 1. Kiểm tra xem hôm nay đã check-in chưa
            var existing = await _attendanceRepository.GetTodayAttendanceAsync(employeeId, today);
            if (existing != null)
            {
                return (false, "Bạn đã thực hiện Check-in cho ngày hôm nay rồi.");
            }

            // 2. Tạo bản ghi mới
            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                Date = today,
                CheckInTime = now.TimeOfDay,
                IsLate = now.TimeOfDay > new TimeSpan(8, 30, 0), // Ví dụ: đi trễ sau 8:30 AM
                Note = "", // Migration của bạn yêu cầu NOT NULL
                TotalHours = 0 // Mặc định khi mới check-in
            };

            await _attendanceRepository.AddAsync(attendance);
            return (true, "Check-in thành công!");
        }

        public async Task<(bool Success, string Message)> CheckOutAsync(int employeeId)
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            // 1. Tìm bản ghi check-in của ngày hôm nay
            var attendance = await _attendanceRepository.GetTodayAttendanceAsync(employeeId, today);
            if (attendance == null)
            {
                return (false, "Không tìm thấy dữ liệu Check-in. Vui lòng Check-in trước.");
            }

            if (attendance.CheckOutTime != null)
            {
                return (false, "Bạn đã thực hiện Check-out rồi.");
            }

            // 2. Cập nhật giờ Check-out
            attendance.CheckOutTime = now.TimeOfDay;

            // 3. Tính toán tổng số giờ làm (TotalHours)
            if (attendance.CheckInTime.HasValue)
            {
                var duration = (attendance.CheckOutTime.Value - attendance.CheckInTime.Value).TotalHours;
                attendance.TotalHours = (decimal)Math.Round(duration, 2);
            }

            await _attendanceRepository.UpdateAsync(attendance);
            return (true, "Check-out thành công!");
        }
    }
}