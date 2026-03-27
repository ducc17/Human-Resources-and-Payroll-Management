using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Payslip;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IPayslipService
    {
        Task<Employee> GetEmployeeIdByIdAsync(int employeeId);
        Task<List<MyPayslipListViewModel>> GetMyPayslipsAsync(int employeeId);
        Task<PayslipDetailViewModel?> GetPayslipDetailAsync(int payslipId);
    }
}
