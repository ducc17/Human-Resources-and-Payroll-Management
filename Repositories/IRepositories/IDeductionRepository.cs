using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Deduction;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IDeductionRepository
    {
        Task<DeductionIndexViewModel> GetAllAsync(string? keyword = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<Deduction?> GetByIdAsync(int id);
        Task CreateAsync(Deduction deduction);
        Task UpdateAsync(Deduction deduction);
        Task DeactivateAsync(int id);
        Task RestoreAsync(int id);
        Task<bool> NameExistsAsync(string name);
        Task<bool> NameExistsAsync(string name, int excludeDeductionId);
    }
}
