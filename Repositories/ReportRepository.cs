using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly DBCodeFirstContext _context;

        public ReportRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        // 🔹 Lấy danh sách nhân viên (có filter theo phòng ban)
        public async Task<List<Employee>> GetEmployeesAsync(int? departmentId)
        {
            var query = _context.Employees
                .Include(e => e.Job)
                    .ThenInclude(j => j.Department)
                .AsQueryable();

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(e => e.Job != null && e.Job.DepartmentId == departmentId);
            }

            return await query.ToListAsync();
        }

        // 🔹 Attendance (lọc theo ngày + phòng ban)
        public async Task<List<Attendance>> GetAttendancesAsync(DateOnly from, DateOnly to, int? departmentId)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Job)
                .AsQueryable();

            query = query.Where(a => a.Date >= from && a.Date <= to);

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(a => a.Employee.Job != null && a.Employee.Job.DepartmentId == departmentId);
            }

            return await query.ToListAsync();
        }

        // 🔹 Leave
        public async Task<List<LeaveRequest>> GetLeavesAsync(DateOnly from, DateOnly to, int? departmentId)
        {
            var query = _context.LeaveRequests
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Job)
                .AsQueryable();

            query = query.Where(l => l.StartDate >= from && l.EndDate <= to);

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(l => l.Employee.Job != null && l.Employee.Job.DepartmentId == departmentId);
            }

            return await query.ToListAsync();
        }

        // 🔹 Payslip
        public async Task<List<Payslip>> GetPayslipsAsync(DateOnly from, DateOnly to, int? departmentId)
        {
            var query = _context.Payslips
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Job)
                .Include(p => p.PayrollPeriod)
                .AsQueryable();

            query = query.Where(p =>
                p.PayrollPeriod.StartDate >= from &&
                p.PayrollPeriod.EndDate <= to);

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(p => p.Employee.Job != null && p.Employee.Job.DepartmentId == departmentId);
            }

            return await query.ToListAsync();
        }

        // Summary nhanh (KHÔNG cần load list)
        public async Task<(decimal totalSalary, decimal totalAllowance, decimal totalDeduction)>
    GetPayrollSummaryAsync(DateOnly from, DateOnly to, int? departmentId)
        {
            var query = _context.Payslips
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Job)
                .Include(p => p.PayrollPeriod)
                .AsQueryable();

            query = query.Where(p =>
                p.PayrollPeriod.StartDate >= from &&
                p.PayrollPeriod.EndDate <= to);

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(p =>
                    p.Employee.Job != null &&
                    p.Employee.Job.DepartmentId == departmentId);
            }

            var list = await query.ToListAsync();

            return (
                totalSalary: list.Sum(p => p.NetSalary),
                totalAllowance: list.Sum(p => p.TotalAllowances),
                totalDeduction: list.Sum(p => p.TotalDeductions)
            );
        }
    }
}