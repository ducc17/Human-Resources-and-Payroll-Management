using SmartHR_Payroll.ViewModels.Insurance;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IInsuranceService
    {
        Task<List<InsuranceViewModel>> GetAllInsurancesAsync();
        Task<InsuranceViewModel?> GetInsuranceByIdAsync(int id);
        Task<bool> CreateInsuranceAsync(InsuranceViewModel model, string createdBy);
        Task<bool> UpdateInsuranceAsync(InsuranceViewModel model, string updatedBy);
        Task DeleteInsuranceAsync(int id, string deletedBy);
    }
}
