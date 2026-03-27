using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IReportRepository
    {
        Task<List<Employee>> GetEmployeesAsync(int? departmentId);
        Task<List<Attendance>> GetAttendancesAsync(DateOnly from, DateOnly to, int? departmentId);
        Task<List<LeaveRequest>> GetLeavesAsync(DateOnly from, DateOnly to, int? departmentId);
        Task<List<Payslip>> GetPayslipsAsync(DateOnly from, DateOnly to, int? departmentId);

        Task<(decimal totalSalary, decimal totalAllowance, decimal totalDeduction)>
            GetPayrollSummaryAsync(DateOnly from, DateOnly to, int? departmentId);
    }
}
