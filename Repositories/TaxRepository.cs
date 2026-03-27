using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class TaxRepository : ITaxRepository
    {
        private readonly DBCodeFirstContext _context;

        public TaxRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<List<TaxBracket>> GetAllAsync()
        {
            return await _context.TaxBrackets.OrderBy(t => t.Level).ToListAsync();
        }

        public async Task<TaxBracket?> GetByIdAsync(int id)
        {
            return await _context.TaxBrackets.FindAsync(id);
        }

        public async Task AddAsync(TaxBracket taxBracket)
        {
            await _context.TaxBrackets.AddAsync(taxBracket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaxBracket taxBracket)
        {
            _context.TaxBrackets.Update(taxBracket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string deletedBy)
        {
            var tax = await _context.TaxBrackets.FindAsync(id);
            if (tax != null)
            {
                tax.IsDeleted = true;
                tax.UpdatedBy = deletedBy;
                await _context.SaveChangesAsync();
            }
        }
    }
}
