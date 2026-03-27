namespace SmartHR_Payroll.ViewModels.Attendance
{
    public class DailyAttendanceViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public DateOnly Date { get; set; }

        // Giờ Check-in sớm nhất trong ngày
        public TimeSpan? FirstCheckIn { get; set; }

        // Giờ Check-out muộn nhất trong ngày
        public TimeSpan? LastCheckOut { get; set; }

        // Tổng thời gian làm việc (Tính từ FirstCheckIn đến LastCheckOut)
        public decimal TotalHours { get; set; }

        // Trạng thái Đi muộn (Dựa vào FirstCheckIn)
        public bool IsLate { get; set; }

        // Logic trạng thái Đi làm chuẩn chỉ (Như chúng ta đã bàn lúc trước)
        public bool IsPerfect => FirstCheckIn.HasValue && LastCheckOut.HasValue && !IsLate;

        // Danh sách chi tiết TẤT CẢ các lần quẹt thẻ trong ngày này
        // (Dùng để hiển thị khi người dùng bấm xem chi tiết)
        public List<SmartHR_Payroll.Models.Attendance> Details { get; set; } = new List<SmartHR_Payroll.Models.Attendance>();
    }
}