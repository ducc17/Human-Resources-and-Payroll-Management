using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface ITaxRepository
    {
        Task<List<TaxBracket>> GetAllAsync();
        Task<TaxBracket?> GetByIdAsync(int id);
        Task AddAsync(TaxBracket taxBracket);
        Task UpdateAsync(TaxBracket taxBracket);
        Task DeleteAsync(int id, string deletedBy);
    }
}
