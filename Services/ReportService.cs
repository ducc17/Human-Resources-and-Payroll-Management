using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels;
using SmartHR_Payroll.ViewModels.Report;

namespace SmartHR_Payroll.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<TeamReportViewModel> GetTeamReportAsync(
    DateOnly fromDate,
    DateOnly toDate,
    int? departmentId)
        {
            var employees = await _repo.GetEmployeesAsync(departmentId);
            var attendances = await _repo.GetAttendancesAsync(fromDate, toDate, departmentId);
            var leaves = await _repo.GetLeavesAsync(fromDate, toDate, departmentId);
            var payslips = await _repo.GetPayslipsAsync(fromDate, toDate, departmentId);

            var payrollSummary = await _repo.GetPayrollSummaryAsync(fromDate, toDate, departmentId);

            var result = new TeamReportViewModel
            {
                TotalEmployees = employees.Count,
                ActiveEmployees = employees.Count(e => e.Status == Models.Status.EmployeeStatus.Active),
                OnLeaveEmployees = employees.Count(e => e.Status == Models.Status.EmployeeStatus.OnLeave),

                TotalSalary = payrollSummary.totalSalary,
                TotalAllowance = payrollSummary.totalAllowance,
                TotalDeduction = payrollSummary.totalDeduction
            };

            foreach (var e in employees)
            {
                var empAttendances = attendances
                    .Where(a => a.EmployeeId == e.EmployeeId)
                    .ToList();

                var empLeaves = leaves
                    .Where(l => l.EmployeeId == e.EmployeeId &&
                                l.Status == Models.Status.LeaveStatus.Approved)
                    .ToList();

                var empPayslips = payslips
                    .Where(p => p.EmployeeId == e.EmployeeId)
                    .ToList();

                decimal totalHours = 0;

                var daily = empAttendances
                    .GroupBy(a => a.Date)
                    .Select(g =>
                    {
                        var firstIn = g.Where(x => x.CheckInTime.HasValue)
                                       .Min(x => x.CheckInTime);

                        var lastOut = g.Where(x => x.CheckOutTime.HasValue)
                                       .Max(x => x.CheckOutTime);

                        decimal hours = 0;

                        if (firstIn.HasValue && lastOut.HasValue)
                        {
                            hours = (decimal)(lastOut.Value - firstIn.Value).TotalHours;
                        }

                        totalHours += hours;

                        bool isLate = firstIn.HasValue && firstIn > new TimeSpan(8, 30, 0);
                        bool isAbsent = !firstIn.HasValue;

                        return new DailyReportItem
                        {
                            Date = g.Key,
                            CheckIn = firstIn,
                            CheckOut = lastOut,
                            Hours = Math.Round(hours, 2),
                            IsLate = isLate,
                            IsAbsent = isAbsent
                        };
                    }).ToList();

                int workDays = daily.Count(x => !x.IsAbsent);
                int lateDays = daily.Count(x => x.IsLate);
                int absentDays = daily.Count(x => x.IsAbsent);

                result.Employees.Add(new EmployeeReportItem
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.FullName,
                    Department = e.Job?.Department?.Name ?? "N/A",

                    TotalHours = Math.Round(totalHours, 2),
                    WorkDays = workDays,
                    LateCount = lateDays,
                    AbsentDays = absentDays,
                    AvgHours = workDays > 0 ? Math.Round(totalHours / workDays, 2) : 0,

                    NetSalary = empPayslips.Sum(p => p.NetSalary),

                    DailyDetails = daily
                });
            }

            return result;
        }
    }
}