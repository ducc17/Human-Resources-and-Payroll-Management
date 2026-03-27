using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IPayrollPeriodRepository
    {
        Task<List<PayrollPeriod>> GetAllPeriodsAsync();
        Task<PayrollPeriod?> GetPeriodByIdAsync(int id);
        Task AddPeriodAsync(PayrollPeriod period);
        Task UpdatePeriodAsync(PayrollPeriod period);

        Task<List<Payslip>> GetPayslipsByPeriodAsync(int periodId);
        Task RemovePayslipsByPeriodAsync(int periodId);

        Task<List<Employee>> GetEligibleEmployeesForPayrollAsync(DateOnly startDate, DateOnly endDate);

        Task<List<Insurance>> GetInsurancesAsync();
        Task<List<TaxBracket>> GetTaxBracketsAsync();
        Task SavePayslipsBulkAsync(List<Payslip> payslips);
    }
}
