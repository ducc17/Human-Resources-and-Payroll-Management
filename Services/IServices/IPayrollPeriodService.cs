using SmartHR_Payroll.ViewModels.PayrollPeriod;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IPayrollPeriodService
    {
        Task<List<PayrollPeriodViewModel>> GetAllPeriodsAsync();
        Task CreatePeriodAsync(PayrollPeriodViewModel model, string createdBy);
        Task<PayrollSheetViewModel?> GetPayrollSheetAsync(int periodId);
        Task<bool> ProcessPayrollAsync(int periodId, string processedBy);
        Task<bool> ApprovePayrollAsync(int periodId, string approvedBy);
    }
}
