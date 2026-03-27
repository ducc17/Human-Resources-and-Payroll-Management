using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class PayslipRepository : IPayslipRepository
    {
        private readonly DBCodeFirstContext _context;

        public PayslipRepository(DBCodeFirstContext context) { _context = context; }

        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<List<Payslip>> GetMyPayslipsAsync(int employeeId)
        {
            return await _context.Payslips
                .Include(p => p.PayrollPeriod)
                .Where(p => p.EmployeeId == employeeId && p.PayrollPeriod.Status == Status.PayrollStatus.Approved)
                .OrderByDescending(p => p.PayrollPeriod.StartDate)
                .ToListAsync();
        }

        public async Task<Payslip?> GetPayslipDetailAsync(int payslipId)
        {
            return await _context.Payslips
                .Include(p => p.PayrollPeriod)
                .Include(p => p.Employee).ThenInclude(e => e.Job).ThenInclude(j => j.Department)
                .Include(p => p.Employee).ThenInclude(e => e.Job).ThenInclude(j => j.Position)
                .Include(p => p.Employee).ThenInclude(e => e.Bank)
                .Include(p => p.Employee).ThenInclude(e => e.Allowances).ThenInclude(a => a.Allowance)
                .Include(p => p.Employee).ThenInclude(e => e.Deductions).ThenInclude(d => d.Deduction)
                .Include(p => p.Employee).ThenInclude(e => e.Attendances)
                .FirstOrDefaultAsync(p => p.PayslipId == payslipId);
        }
    }
}
