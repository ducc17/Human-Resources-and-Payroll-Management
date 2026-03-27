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
    }
}
