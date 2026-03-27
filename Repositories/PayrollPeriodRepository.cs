using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class PayrollPeriodRepository : IPayrollPeriodRepository
    {
        private readonly DBCodeFirstContext _context;

        public PayrollPeriodRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<List<PayrollPeriod>> GetAllPeriodsAsync()
        {
            return await _context.PayrollPeriods
                .Include(p => p.Payslips)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<PayrollPeriod?> GetPeriodByIdAsync(int id)
        {
            return await _context.PayrollPeriods.FindAsync(id);
        }

        public async Task AddPeriodAsync(PayrollPeriod period)
        {
            await _context.PayrollPeriods.AddAsync(period);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePeriodAsync(PayrollPeriod period)
        {
            _context.PayrollPeriods.Update(period);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Payslip>> GetPayslipsByPeriodAsync(int periodId)
        {
            return await _context.Payslips
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Job)
                        .ThenInclude(j => j.Department)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Allowances)
                        .ThenInclude(ea => ea.Allowance)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Deductions)
                        .ThenInclude(ed => ed.Deduction)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Attendances)
                .Where(p => p.PayrollPeriodId == periodId)
                .ToListAsync();
        }

        public async Task RemovePayslipsByPeriodAsync(int periodId)
        {
            var oldPayslips = await _context.Payslips.Where(p => p.PayrollPeriodId == periodId).ToListAsync();
            if (oldPayslips.Any())
            {
                _context.Payslips.RemoveRange(oldPayslips);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Employee>> GetEligibleEmployeesForPayrollAsync()
        {
            return await _context.Employees
                .Include(e => e.Contracts.Where(c => c.IsActive && !c.IsDeleted))
                .Include(e => e.Allowances)
                .Include(e => e.Deductions)
                .Where(e => e.Status == Status.EmployeeStatus.Active)
                .ToListAsync();
        }

        public async Task SavePayslipsBulkAsync(List<Payslip> payslips)
        {
            await _context.Payslips.AddRangeAsync(payslips);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Insurance>> GetInsurancesAsync() => await _context.Insurances.ToListAsync();
        public async Task<List<TaxBracket>> GetTaxBracketsAsync() => await _context.TaxBrackets.OrderByDescending(t => t.Level).ToListAsync();
        public async Task<List<Employee>> GetEligibleEmployeesForPayrollAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.Employees
                .Include(e => e.Contracts.Where(c => c.IsActive))

                .Include(e => e.Allowances)
                    .ThenInclude(ea => ea.Allowance)

                .Include(e => e.Deductions)

                .Include(e => e.Attendances.Where(a => a.Date >= startDate && a.Date <= endDate)) // Note

                .Where(e => e.Status == Status.EmployeeStatus.Active)
                .ToListAsync();
        }

    }
}
