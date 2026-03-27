using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Deduction;

namespace SmartHR_Payroll.Services
{
    public class DeductionService : IDeductionService
    {
        private readonly IDeductionRepository _deductionRepository;

        public DeductionService(IDeductionRepository deductionRepository)
        {
            _deductionRepository = deductionRepository;
        }

        public async Task<DeductionIndexViewModel> GetAllAsync(string? keyword = null, string? sortBy = null, int page = 1, int pageSize = 10)
            => await _deductionRepository.GetAllAsync(keyword, sortBy, page, pageSize);

        public async Task<Deduction?> GetByIdAsync(int id)
            => await _deductionRepository.GetByIdAsync(id);

        public async Task CreateAsync(Deduction deduction, string currentUserName)
        {
            deduction.IsDeleted = false;
            deduction.CreatedBy = currentUserName;
            deduction.CreatedAt = DateTime.UtcNow;
            deduction.UpdatedBy = currentUserName;
            deduction.UpdatedAt = DateTime.UtcNow;

            await _deductionRepository.CreateAsync(deduction);
        }

        public async Task UpdateAsync(Deduction deduction, string currentUserName)
        {
            deduction.UpdatedBy = currentUserName;
            deduction.UpdatedAt = DateTime.UtcNow;

            await _deductionRepository.UpdateAsync(deduction);
        }

        public async Task DeactivateAsync(int id)
            => await _deductionRepository.DeactivateAsync(id);

        public async Task RestoreAsync(int id)
            => await _deductionRepository.RestoreAsync(id);

        public async Task<bool> NameExistsAsync(string name)
            => await _deductionRepository.NameExistsAsync(name);

        public async Task<bool> NameExistsAsync(string name, int excludeDeductionId)
            => await _deductionRepository.NameExistsAsync(name, excludeDeductionId);
    }
}
