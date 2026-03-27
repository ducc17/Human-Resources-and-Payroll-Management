using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.TaxBracket;

namespace SmartHR_Payroll.Services
{
    public class TaxService : ITaxService
    {
        private readonly ITaxRepository _taxRepository;

        public TaxService(ITaxRepository taxRepository)
        {
            _taxRepository = taxRepository;
        }

        public async Task<List<TaxBracketViewModel>> GetAllTaxBracketsAsync()
        {
            var list = await _taxRepository.GetAllAsync();
            return list.Select(t => new TaxBracketViewModel
            {
                TaxBracketId = t.TaxBracketId,
                Level = t.Level,
                FromIncome = t.FromIncome,
                ToIncome = t.ToIncome,
                TaxRate = t.TaxRate,
                QuickSubtraction = t.QuickSubtraction
            }).ToList();
        }

        public async Task<TaxBracketViewModel?> GetTaxBracketByIdAsync(int id)
        {
            var t = await _taxRepository.GetByIdAsync(id);
            if (t == null) return null;
            return new TaxBracketViewModel { TaxBracketId = t.TaxBracketId, Level = t.Level, FromIncome = t.FromIncome, ToIncome = t.ToIncome, TaxRate = t.TaxRate, QuickSubtraction = t.QuickSubtraction };
        }

        public async Task CreateTaxBracketAsync(TaxBracketViewModel model, string createdBy)
        {
            var entity = new TaxBracket { Level = model.Level, FromIncome = model.FromIncome, ToIncome = model.ToIncome, TaxRate = model.TaxRate, QuickSubtraction = model.QuickSubtraction, CreatedBy = createdBy };
            await _taxRepository.AddAsync(entity);
        }

        public async Task UpdateTaxBracketAsync(TaxBracketViewModel model, string updatedBy)
        {
            var entity = await _taxRepository.GetByIdAsync(model.TaxBracketId);
            if (entity != null)
            {
                entity.Level = model.Level;
                entity.FromIncome = model.FromIncome;
                entity.ToIncome = model.ToIncome;
                entity.TaxRate = model.TaxRate;
                entity.QuickSubtraction = model.QuickSubtraction;
                entity.UpdatedBy = updatedBy;
                await _taxRepository.UpdateAsync(entity);
            }
        }

        public async Task DeleteTaxBracketAsync(int id, string deletedBy) => await _taxRepository.DeleteAsync(id, deletedBy);
    }
}
