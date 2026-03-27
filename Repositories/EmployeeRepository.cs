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

        public async Task<(List<Employee> Employees, int TotalCount)> GetEmployeesPagedAsync(int page, int pageSize)
        {
            var query = _context.Employees
                .IgnoreQueryFilters()
                .Include(e => e.Bank)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Department)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Position)
                .OrderByDescending(e => e.CreatedAt)
                .AsQueryable();

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
                .Include(e => e.Job)
                    .ThenInclude(j => j.Department)
                .Include(e => e.Job)
                    .ThenInclude(j => j.Position)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }
        public async Task<List<Bank>> GetAllBanksAsync()
        {
            return await _context.Banks.Where(b => !b.IsDeleted).ToListAsync();
        }
    }
}