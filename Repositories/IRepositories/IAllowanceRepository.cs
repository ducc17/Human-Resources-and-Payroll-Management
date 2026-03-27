using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Allowance;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IAllowanceRepository
    {
        Task<AllowanceIndexViewModel> GetAllAsync(string? keyword = null, bool? isTaxable = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<Allowance?> GetByIdAsync(int id);
        Task CreateAsync(Allowance allowance);
        Task UpdateAsync(Allowance allowance);
        Task DeactivateAsync(int id);
        Task RestoreAsync(int id);
        Task<bool> NameExistsAsync(string name);
        Task<bool> NameExistsAsync(string name, int excludeAllowanceId);
    }
}
