using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly DBCodeFirstContext _context;

        public InsuranceRepository(DBCodeFirstContext context)
        {
            _context = context;
        }
        public async Task<List<Insurance>> GetAllAsync()
        {
            return await _context.Insurances.OrderBy(i => i.Code).ToListAsync();
        }

        public async Task<Insurance?> GetByIdAsync(int id)
        {
            return await _context.Insurances.FindAsync(id);
        }

        public async Task<bool> CheckCodeExistsAsync(string code, int excludeId = 0)
        {
            return await _context.Insurances.AnyAsync(i => i.Code == code && i.InsuranceId != excludeId);
        }

        public async Task AddAsync(Insurance insurance)
        {
            await _context.Insurances.AddAsync(insurance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Insurance insurance)
        {
            _context.Insurances.Update(insurance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string deletedBy)
        {
            var insurance = await _context.Insurances.FindAsync(id);
            if (insurance != null)
            {
                insurance.IsDeleted = true;
                insurance.UpdatedBy = deletedBy;
                await _context.SaveChangesAsync();
            }
        }
    }
}
