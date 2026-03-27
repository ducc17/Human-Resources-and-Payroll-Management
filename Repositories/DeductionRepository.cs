using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.ViewModels.Deduction;

namespace SmartHR_Payroll.Repositories
{
    public class DeductionRepository : IDeductionRepository
    {
        private readonly DBCodeFirstContext _context;

        private static string NormalizeName(string? name)
            => string.Join(" ", (name ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToLowerInvariant();

        public DeductionRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<DeductionIndexViewModel> GetAllAsync(string? keyword = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _context.Deductions.IgnoreQueryFilters()
                .Where(d => string.IsNullOrWhiteSpace(keyword) || d.Name.Contains(keyword))
                .Select(d => new DeductionListViewModel
                {
                    DeductionId = d.DeductionId,
                    Name = d.Name,
                    IsDeleted = d.IsDeleted,
                    EmployeeCount = _context.EmployeeDeductions.Count(ed => ed.DeductionId == d.DeductionId),
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt
                });

            if (sortBy == "emp_desc")
            {
                query = query.OrderByDescending(d => d.EmployeeCount).ThenBy(d => d.Name);
            }
            else if (sortBy == "emp_asc")
            {
                query = query.OrderBy(d => d.EmployeeCount).ThenBy(d => d.Name);
            }
            else
            {
                query = query.OrderBy(d => d.IsDeleted).ThenBy(d => d.Name);
            }

            var totalCount = await query.CountAsync();
            var deductions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new DeductionIndexViewModel
            {
                Deductions = deductions,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<Deduction?> GetByIdAsync(int id)
            => await _context.Deductions.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.DeductionId == id);

        public async Task CreateAsync(Deduction deduction)
        {
            await _context.Deductions.AddAsync(deduction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Deduction deduction)
        {
            var existingDeduction = await _context.Deductions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.DeductionId == deduction.DeductionId);

            if (existingDeduction == null) return;

            existingDeduction.Name = deduction.Name;
            existingDeduction.UpdatedAt = deduction.UpdatedAt;
            existingDeduction.UpdatedBy = deduction.UpdatedBy;

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var deduction = await _context.Deductions.FindAsync(id);
            if (deduction == null) return;

            deduction.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var deduction = await _context.Deductions.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.DeductionId == id);
            if (deduction == null) return;

            deduction.IsDeleted = false;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            var normalizedName = NormalizeName(name);
            var names = await _context.Deductions
                .IgnoreQueryFilters()
                .Select(d => d.Name)
                .ToListAsync();

            return names.Any(n => NormalizeName(n) == normalizedName);
        }

        public async Task<bool> NameExistsAsync(string name, int excludeDeductionId)
        {
            var normalizedName = NormalizeName(name);
            var names = await _context.Deductions
                .IgnoreQueryFilters()
                .Where(d => d.DeductionId != excludeDeductionId)
                .Select(d => d.Name)
                .ToListAsync();

            return names.Any(n => NormalizeName(n) == normalizedName);
        }
    }
}
