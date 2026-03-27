using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DBCodeFirstContext _context;

        public EmployeeRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeProfileAsync(int employeeId)
        {
            return await _context.Employees
                .Include(e => e.Bank)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Department)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Position)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees
                .IgnoreQueryFilters()
                .Include(e => e.Bank)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Department)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Position)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetEmployeesPagedAsync(
             int page,
             int pageSize,
             int? departmentId,
             string keyword,
             string status)
        {
            var query = _context.Employees
                .IgnoreQueryFilters()
                .Include(e => e.Bank)
                .Include(e => e.Role)
                .Include(e => e.Job).ThenInclude(j => j.Department)
                .Include(e => e.Job).ThenInclude(j => j.Position)
                .AsQueryable();

            // Manager filter
            if (departmentId.HasValue)
            {
                query = query.Where(e => e.Job.DepartmentId == departmentId.Value);
            }

            // SEARCH
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(e =>
                    e.EmployeeCode.Contains(keyword) ||
                    (e.FirstName + " " + e.LastName).Contains(keyword) ||
                    e.Email.Contains(keyword) ||
                    e.PhoneNumber.Contains(keyword)
                );
            }

            // STATUS FILTER
            if (!string.IsNullOrEmpty(status) &&
                Enum.TryParse<Status.EmployeeStatus>(status, out var parsedStatus))
            {
                query = query.Where(e => e.Status == parsedStatus);
            }

            query = query.OrderByDescending(e => e.CreatedAt);

            var totalCount = await query.CountAsync();

            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalCount);
        }

        public async Task<List<Contract>> GetContractsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Contracts
                .IgnoreQueryFilters()
                .Where(c => c.EmployeeId == employeeId)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<bool> SoftDeleteEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmployeeCodeExistsAsync(string employeeCode)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeCode == employeeCode);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await _context.Employees.AnyAsync(e => e.PhoneNumber == phoneNumber);
        }

        public async Task<bool> BankAccountNumberExistsAsync(string bankAccountNumber)
        {
            return await _context.Employees.AnyAsync(e => e.BankAccountNumber == bankAccountNumber);
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<Position>> GetPositionsAsync()
        {
            return await _context.Positions
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Role
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<Job>> GetJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Department)
                .Include(j => j.Position)
                .OrderBy(j => j.Department.Name)
                .ThenBy(j => j.Position.Name)
                .ToListAsync();
        }

        public async Task<List<Bank>> GetBanksAsync()
        {
            return await _context.Banks
                .OrderBy(b => b.BankName)
                .ToListAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Bank)
                .Include(e => e.Role)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Department)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Position)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<List<Allowance>> GetActiveAllowancesAsync()
        {
            return await _context.Allowances
                .IgnoreQueryFilters()
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<List<EmployeeAllowance>> GetEmployeeAllowancesAsync(int employeeId)
        {
            return await _context.EmployeeAllowances
                .Include(ea => ea.Allowance)
                .Where(ea => ea.EmployeeId == employeeId)
                .OrderByDescending(ea => ea.EffectiveDate)
                .ThenBy(ea => ea.Allowance.Name)
                .ToListAsync();
        }

        public async Task<EmployeeAllowance?> GetEmployeeAllowanceAsync(int employeeId, int allowanceId, DateOnly effectiveDate)
        {
            return await _context.EmployeeAllowances
                .FirstOrDefaultAsync(ea => ea.EmployeeId == employeeId
                                           && ea.AllowanceId == allowanceId
                                           && ea.EffectiveDate == effectiveDate);
        }

        public async Task SaveEmployeeAllowanceAsync(EmployeeAllowance employeeAllowance)
        {
            try
            {
                if (employeeAllowance == null)
                {
                    throw new ArgumentNullException(nameof(employeeAllowance));
                }

                System.Diagnostics.Debug.WriteLine($"SaveEmployeeAllowanceAsync: EmployeeId={employeeAllowance.EmployeeId}, AllowanceId={employeeAllowance.AllowanceId}, Amount={employeeAllowance.Amount}, EffectiveDate={employeeAllowance.EffectiveDate}");

                if (employeeAllowance.Id == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Adding new EmployeeAllowance...");
                    await _context.EmployeeAllowances.AddAsync(employeeAllowance);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Updating existing EmployeeAllowance (Id={employeeAllowance.Id})...");
                    _context.EmployeeAllowances.Update(employeeAllowance);
                }

                var changeCount = await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"SaveChangesAsync completed. Changes saved: {changeCount}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in SaveEmployeeAllowanceAsync: {ex.Message}\n{ex.StackTrace}");
                throw new InvalidOperationException($"Lỗi khi lưu phụ cấp nhân viên: {ex.Message}", ex);
            }
        }

        public async Task<List<Deduction>> GetActiveDeductionsAsync()
        {
            return await _context.Deductions
                .IgnoreQueryFilters()
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<EmployeeDeduction>> GetEmployeeDeductionsAsync(int employeeId)
        {
            return await _context.EmployeeDeductions
                .Include(ed => ed.Deduction)
                .Where(ed => ed.EmployeeId == employeeId)
                .OrderByDescending(ed => ed.EffectiveDate)
                .ThenBy(ed => ed.Deduction.Name)
                .ToListAsync();
        }

        public async Task<EmployeeDeduction?> GetEmployeeDeductionAsync(int employeeId, int deductionId, DateOnly effectiveDate)
        {
            return await _context.EmployeeDeductions
                .FirstOrDefaultAsync(ed => ed.EmployeeId == employeeId
                                           && ed.DeductionId == deductionId
                                           && ed.EffectiveDate == effectiveDate);
        }

        public async Task SaveEmployeeDeductionAsync(EmployeeDeduction employeeDeduction)
        {
            try
            {
                if (employeeDeduction == null)
                {
                    throw new ArgumentNullException(nameof(employeeDeduction));
                }

                if (employeeDeduction.Id == 0)
                {
                    await _context.EmployeeDeductions.AddAsync(employeeDeduction);
                }
                else
                {
                    _context.EmployeeDeductions.Update(employeeDeduction);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lưu khoản trừ nhân viên: {ex.Message}", ex);
            }
        }
    }
}