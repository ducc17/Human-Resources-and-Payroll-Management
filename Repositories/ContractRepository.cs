using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly DBCodeFirstContext _context;

        public ContractRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetEmployeesForContractAsync()
        {
            return await _context.Employees
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == employeeId && !e.IsDeleted);
        }

        public async Task<bool> ContractNumberExistsAsync(string contractNumber)
        {
            return await _context.Contracts.AnyAsync(c => c.ContractNumber == contractNumber && !c.IsDeleted);
        }

        public async Task AddContractAsync(Contract contract)
        {
            await _context.Contracts.AddAsync(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<Contract?> GetContractByIdAsync(int contractId)
        {
            return await _context.Contracts
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task UpdateContractAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveContractAsync(int employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Contracts.AnyAsync(c =>
                c.EmployeeId == employeeId &&
                !c.IsDeleted &&
                c.IsActive &&
                (!c.EndDate.HasValue || c.EndDate.Value >= today)
            );
        }
    }
}
