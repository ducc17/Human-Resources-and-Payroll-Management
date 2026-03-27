namespace SmartHR_Payroll.ViewModels.Attendance
{
    public class AttendanceHistoryViewModel
    {
        public DateOnly Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public decimal TotalHours { get; set; }
        public bool IsLate { get; set; }
        public string? Note { get; set; }

        public bool IsPerfect => CheckInTime.HasValue && CheckOutTime.HasValue && !IsLate;
    }
}