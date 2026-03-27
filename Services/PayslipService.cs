using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Payslip;

namespace SmartHR_Payroll.Services
{
    public class PayslipService : IPayslipService
    {
        private readonly IPayslipRepository _repo;

        public PayslipService(IPayslipRepository repo) { _repo = repo; }

        public async Task<Employee> GetEmployeeIdByIdAsync(int employeeId)
        {
            var emp = await _repo.GetEmployeeByIdAsync(employeeId);
            return emp;
        }

        public async Task<List<MyPayslipListViewModel>> GetMyPayslipsAsync(int employeeId)
        {
            var list = await _repo.GetMyPayslipsAsync(employeeId);
            return list.Select(p => new MyPayslipListViewModel
            {
                PayslipId = p.PayslipId,
                PeriodName = p.PayrollPeriod.Name,
                StartDate = p.PayrollPeriod.StartDate,
                EndDate = p.PayrollPeriod.EndDate,
                NetSalary = p.NetSalary,
                PaymentDate = p.PaymentDate,
                Status = p.PaymentDate.HasValue ? "Đã thanh toán" : "Chưa thanh toán"
            }).ToList();
        }

        public async Task<PayslipDetailViewModel?> GetPayslipDetailAsync(int payslipId)
        {
            var p = await _repo.GetPayslipDetailAsync(payslipId);
            if (p == null) return null;

            var allowanceDetails = p.Employee.Allowances
                .Where(a => p.PayrollPeriod.StartDate <= a.EffectiveDate && a.EffectiveDate <= p.PayrollPeriod.EndDate)
                .Select(a => new PayslipAllowanceDetail { Name = a.Allowance.Name, Amount = a.Amount }).ToList();

            var deductionDetails = p.Employee.Deductions
                .Where(d => p.PayrollPeriod.StartDate <= d.EffectiveDate && d.EffectiveDate <= p.PayrollPeriod.EndDate)
                .Select(d => new PayslipDeductionDetail { Name = d.Deduction.Name, Amount = d.Amount }).ToList();

            return new PayslipDetailViewModel
            {
                PayslipId = p.PayslipId,
                PeriodName = p.PayrollPeriod.Name,
                StartDate = p.PayrollPeriod.StartDate,
                EndDate = p.PayrollPeriod.EndDate,

                EmployeeCode = p.Employee.EmployeeCode,
                FullName = p.Employee.FirstName + " " + p.Employee.LastName,
                DepartmentName = p.Employee.Job?.Department?.Name ?? "N/A",
                PositionName = p.Employee.Job?.Position?.Name ?? "N/A",
                BankName = p.Employee.Bank?.ShortName ?? "Tiền mặt",
                BankAccountNumber = p.Employee.BankAccountNumber ?? "N/A",

                WorkingDays = p.WorkingDays,
                PaidLeaveDays = p.PaidLeaveDays,
                BaseSalary = p.BaseSalary,
                TotalAllowances = p.TotalAllowances,
                SocialInsuranceAmount = p.SocialInsuranceAmount,
                TaxAmount = p.TaxAmount,
                OtherDeductions = p.OtherDeductions,
                NetSalary = p.NetSalary,
                PaymentDate = p.PaymentDate,
                Remarks = p.Remarks,

                AllowanceDetails = allowanceDetails,
                DeductionDetails = deductionDetails
            };
        }
    }
}
