namespace SmartHR_Payroll.ViewModels.Report
{
    public class TeamReportViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int OnLeaveEmployees { get; set; }

        public decimal TotalSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalDeduction { get; set; }

        public List<EmployeeReportItem> Employees { get; set; } = new();
    }

    public class EmployeeReportItem
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        public string Department { get; set; }

        public decimal TotalHours { get; set; }
        public int WorkDays { get; set; }
        public int LateCount { get; set; }
        public int AbsentDays { get; set; }

        public decimal AvgHours { get; set; }

        public decimal NetSalary { get; set; }

        public List<DailyReportItem> DailyDetails { get; set; } = new();
    }

    public class DailyReportItem
    {
        public DateOnly Date { get; set; }

        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }

        public decimal Hours { get; set; }

        public bool IsLate { get; set; }
        public bool IsAbsent { get; set; }
    }
}