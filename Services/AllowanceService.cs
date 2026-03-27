using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Allowance;

namespace SmartHR_Payroll.Services
{
    public class AllowanceService : IAllowanceService
    {
        private readonly IAllowanceRepository _allowanceRepository;

        public AllowanceService(IAllowanceRepository allowanceRepository)
        {
            _allowanceRepository = allowanceRepository;
        }

        public async Task<AllowanceIndexViewModel> GetAllAsync(string? keyword = null, bool? isTaxable = null, string? sortBy = null, int page = 1, int pageSize = 10)
            => await _allowanceRepository.GetAllAsync(keyword, isTaxable, sortBy, page, pageSize);

        public async Task<Allowance?> GetByIdAsync(int id)
            => await _allowanceRepository.GetByIdAsync(id);

        public async Task CreateAsync(Allowance allowance, string currentUserName)
        {
            allowance.IsDeleted = false;
            allowance.CreatedBy = currentUserName;
            allowance.CreatedAt = DateTime.UtcNow;
            allowance.UpdatedBy = currentUserName;
            allowance.UpdatedAt = DateTime.UtcNow;

            await _allowanceRepository.CreateAsync(allowance);
        }

        public async Task UpdateAsync(Allowance allowance, string currentUserName)
        {
            allowance.UpdatedBy = currentUserName;
            allowance.UpdatedAt = DateTime.UtcNow;

            await _allowanceRepository.UpdateAsync(allowance);
        }

        public async Task DeactivateAsync(int id)
            => await _allowanceRepository.DeactivateAsync(id);

        public async Task RestoreAsync(int id)
            => await _allowanceRepository.RestoreAsync(id);

        public async Task<bool> NameExistsAsync(string name)
            => await _allowanceRepository.NameExistsAsync(name);

        public async Task<bool> NameExistsAsync(string name, int excludeAllowanceId)
            => await _allowanceRepository.NameExistsAsync(name, excludeAllowanceId);
    }
}
