using SmartHR_Payroll.ViewModels.TaxBracket;

namespace SmartHR_Payroll.Services.IServices
{
    public interface ITaxService
    {
        Task<List<TaxBracketViewModel>> GetAllTaxBracketsAsync();
        Task<TaxBracketViewModel?> GetTaxBracketByIdAsync(int id);
        Task CreateTaxBracketAsync(TaxBracketViewModel model, string createdBy);
        Task UpdateTaxBracketAsync(TaxBracketViewModel model, string updatedBy);
        Task DeleteTaxBracketAsync(int id, string deletedBy);
    }
}
