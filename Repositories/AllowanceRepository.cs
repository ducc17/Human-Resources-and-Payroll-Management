using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.ViewModels.Allowance;

namespace SmartHR_Payroll.Repositories
{
    public class AllowanceRepository : IAllowanceRepository
    {
        private readonly DBCodeFirstContext _context;

        private static string NormalizeName(string? name)
            => string.Join(" ", (name ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToLowerInvariant();

        public AllowanceRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<AllowanceIndexViewModel> GetAllAsync(string? keyword = null, bool? isTaxable = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _context.Allowances.IgnoreQueryFilters()
                .Where(a => string.IsNullOrWhiteSpace(keyword) || a.Name.Contains(keyword))
                .Where(a => !isTaxable.HasValue || a.IsTaxable == isTaxable.Value)
                .Select(a => new AllowanceListViewModel
                {
                    AllowanceId = a.AllowanceId,
                    Name = a.Name,
                    IsTaxable = a.IsTaxable,
                    IsDeleted = a.IsDeleted,
                    EmployeeCount = _context.EmployeeAllowances.Count(ea => ea.AllowanceId == a.AllowanceId),
                    CreatedBy = a.CreatedBy,
                    CreatedAt = a.CreatedAt
                });

            if (sortBy == "emp_desc")
            {
                query = query.OrderByDescending(a => a.EmployeeCount).ThenBy(a => a.Name);
            }
            else if (sortBy == "emp_asc")
            {
                query = query.OrderBy(a => a.EmployeeCount).ThenBy(a => a.Name);
            }
            else
            {
                query = query.OrderBy(a => a.IsDeleted).ThenBy(a => a.Name);
            }

            var totalCount = await query.CountAsync();
            var allowances = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new AllowanceIndexViewModel
            {
                Allowances = allowances,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<Allowance?> GetByIdAsync(int id)
            => await _context.Allowances.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.AllowanceId == id);

        public async Task CreateAsync(Allowance allowance)
        {
            await _context.Allowances.AddAsync(allowance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Allowance allowance)
        {
            var existingAllowance = await _context.Allowances
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.AllowanceId == allowance.AllowanceId);

            if (existingAllowance == null) return;

            existingAllowance.Name = allowance.Name;
            existingAllowance.IsTaxable = allowance.IsTaxable;
            existingAllowance.UpdatedAt = allowance.UpdatedAt;
            existingAllowance.UpdatedBy = allowance.UpdatedBy;

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var allowance = await _context.Allowances.FindAsync(id);
            if (allowance == null) return;

            allowance.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var allowance = await _context.Allowances.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.AllowanceId == id);
            if (allowance == null) return;

            allowance.IsDeleted = false;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            var normalizedName = NormalizeName(name);
            var names = await _context.Allowances
                .IgnoreQueryFilters()
                .Select(a => a.Name)
                .ToListAsync();

            return names.Any(n => NormalizeName(n) == normalizedName);
        }

        public async Task<bool> NameExistsAsync(string name, int excludeAllowanceId)
        {
            var normalizedName = NormalizeName(name);
            var names = await _context.Allowances
                .IgnoreQueryFilters()
                .Where(a => a.AllowanceId != excludeAllowanceId)
                .Select(a => a.Name)
                .ToListAsync();

            return names.Any(n => NormalizeName(n) == normalizedName);
        }
    }
}
