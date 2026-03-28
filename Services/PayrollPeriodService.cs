using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.PayrollPeriod;

namespace SmartHR_Payroll.Services
{
    public class PayrollPeriodService : IPayrollPeriodService
    {
        private readonly IPayrollPeriodRepository _repo;
        private const decimal STANDARD_WORKING_DAYS = 22m;

        public PayrollPeriodService(IPayrollPeriodRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PayrollPeriodViewModel>> GetAllPeriodsAsync()
        {
            var periods = await _repo.GetAllPeriodsAsync();
            return periods.Select(p => new PayrollPeriodViewModel
            {
                PayrollPeriodId = p.PayrollPeriodId,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                TotalEmployees = p.Payslips != null ? p.Payslips.Count : 0,
                TotalNetSalary = p.Payslips != null ? p.Payslips.Sum(ps => ps.NetSalary) : 0
            }).ToList();
        }

        public async Task CreatePeriodAsync(PayrollPeriodViewModel model, string createdBy)
        {
            var entity = new PayrollPeriod
            {
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = Status.PayrollStatus.Draft,
                CreatedBy = createdBy
            };
            await _repo.AddPeriodAsync(entity);
        }

        public async Task<PayrollSheetViewModel?> GetPayrollSheetAsync(int periodId)
        {
            var period = await _repo.GetPeriodByIdAsync(periodId);
            if (period == null) return null;

            var payslips = await _repo.GetPayslipsByPeriodAsync(periodId);

            var viewModel = new PayrollSheetViewModel
            {
                Period = new PayrollPeriodViewModel
                {
                    PayrollPeriodId = period.PayrollPeriodId,
                    Name = period.Name,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    Status = period.Status
                },
                Payslips = payslips.Select(p =>
                {
                    // Lọc phụ cấp của nhân viên này
                    var allowances = p.Employee.Allowances
                        .Where(a => period.StartDate <= a.EffectiveDate && a.EffectiveDate <= period.EndDate)
                        .Select(a => new AllowanceDetailViewModel { Name = a.Allowance.Name, Amount = a.Amount }).ToList();

                    // Lọc khấu trừ của nhân viên này
                    var deductions = p.Employee.Deductions
                        .Where(d => period.StartDate <= d.EffectiveDate && d.EffectiveDate <= period.EndDate)
                        .Select(d => new DeductionDetailViewModel { Name = d.Deduction.Name, Amount = d.Amount }).ToList();

                    return new PayslipItemViewModel
                    {
                        PayslipId = p.PayslipId,
                        EmployeeId = p.EmployeeId,
                        EmployeeCode = p.Employee.EmployeeCode,
                        FullName = p.Employee.FirstName + " " + p.Employee.LastName,
                        DepartmentName = p.Employee.Job?.Department?.Name ?? "N/A",
                        WorkingDays = p.WorkingDays,
                        BaseSalary = p.BaseSalary,
                        TotalAllowances = p.TotalAllowances,
                        SocialInsuranceAmount = p.SocialInsuranceAmount,
                        TaxAmount = p.TaxAmount,
                        OtherDeductions = p.OtherDeductions,
                        NetSalary = p.NetSalary,

                        AllowanceDetails = allowances,
                        DeductionDetails = deductions
                    };
                }).ToList()
            };

            return viewModel;
        }

        public async Task<bool> ProcessPayrollAsync(int periodId, string processedBy)
        {
            var period = await _repo.GetPeriodByIdAsync(periodId);
            if (!IsValidPeriod(period)) return false;

            await _repo.RemovePayslipsByPeriodAsync(periodId);

            var insurances = await _repo.GetInsurancesAsync();
            var taxBrackets = await _repo.GetTaxBracketsAsync();
            var employees = await _repo.GetEligibleEmployeesForPayrollAsync(period.StartDate, period.EndDate);

            var payslips = new List<Payslip>();

            foreach (var emp in employees)
            {
                var contract = GetActiveContract(emp);
                if (contract == null) continue;

                var workingData = CalculateWorkingDays(emp);
                var salary = CalculateBaseSalary(contract.BaseSalary, workingData.ActualWorkingDays);

                var allowanceData = CalculateAllowances(emp, period);
                var deduction = CalculateDeductions(emp, period);

                var insurance = CalculateInsurance(contract.BaseSalary, insurances);

                var tax = CalculateTax(
                    emp,
                    salary,
                    allowanceData.TaxableAllowances,
                    insurance,
                    taxBrackets
                );

                var netSalary = CalculateNetSalary(
                    salary,
                    allowanceData.TotalAllowances,
                    insurance,
                    tax,
                    deduction
                );

                payslips.Add(CreatePayslip(
                    emp,
                    periodId,
                    workingData,
                    salary,
                    allowanceData.TotalAllowances,
                    insurance,
                    tax,
                    deduction,
                    netSalary,
                    processedBy
                ));
            }

            await _repo.SavePayslipsBulkAsync(payslips);
            await UpdatePeriod(period, processedBy);

            return true;
        }

        private bool IsValidPeriod(PayrollPeriod? period)
        {
            return period != null && period.Status != Status.PayrollStatus.Approved;
        }
        private Contract? GetActiveContract(Employee emp)
        {
            return emp.Contracts.FirstOrDefault();
        }
        private (decimal ActualWorkingDays, int LateCount) CalculateWorkingDays(Employee emp)
        {
            decimal days = 0;
            int late = 0;

            foreach (var att in emp.Attendances)
            {
                if (att.TotalHours >= 8) days += 1;
                else if (att.TotalHours >= 4) days += 0.5m;

                if (att.IsLate) late++;
            }

            return (days, late);
        }
        private decimal CalculateBaseSalary(decimal baseSalary, decimal workingDays)
        {
            return (baseSalary / STANDARD_WORKING_DAYS) * workingDays;
        }
        private (decimal TaxableAllowances, decimal TotalAllowances) CalculateAllowances(Employee emp, PayrollPeriod period)
        {
            var valid = emp.Allowances
                .Where(a => a.EffectiveDate >= period.StartDate && a.EffectiveDate <= period.EndDate)
                .ToList();

            var taxable = valid.Where(a => a.Allowance.IsTaxable).Sum(a => a.Amount);
            var nonTaxable = valid.Where(a => !a.Allowance.IsTaxable).Sum(a => a.Amount);

            return (taxable, taxable + nonTaxable);
        }
        private decimal CalculateDeductions(Employee emp, PayrollPeriod period)
        {
            var deductions = emp.Deductions
                .Where(d => period.StartDate <= d.EffectiveDate && d.EffectiveDate <= period.EndDate)
                .Sum(d => d.Amount);

            return deductions;
        }
        private decimal CalculateInsurance(decimal baseSalary, List<Insurance> insurances)
        {
            decimal total = 0;

            foreach (var ins in insurances)
            {
                var salary = ins.MaxSalaryLimit.HasValue && baseSalary > ins.MaxSalaryLimit
                    ? ins.MaxSalaryLimit.Value
                    : baseSalary;

                total += salary * (ins.EmployeeRate / 100m);
            }

            return total;
        }
        private decimal CalculateTax(
    Employee emp,
    decimal baseSalary,
    decimal taxableAllowances,
    decimal insurance,
    List<TaxBracket> brackets)
        {
            decimal personal = 11000000m;
            decimal dependent = emp.DependentCount * 4400000m;

            var income = baseSalary + taxableAllowances - insurance - personal - dependent;

            if (income <= 0) return 0;

            var bracket = brackets.FirstOrDefault(t => income > t.FromIncome);
            if (bracket == null) return 0;

            return (income * (bracket.TaxRate / 100m)) - bracket.QuickSubtraction;
        }
        private decimal CalculateNetSalary(
    decimal baseSalary,
    decimal allowances,
    decimal insurance,
    decimal tax,
    decimal deductions)
        {
            return baseSalary + allowances - insurance - tax - deductions;
        }
        private Payslip CreatePayslip(
    Employee emp,
    int periodId,
    (decimal ActualWorkingDays, int LateCount) work,
    decimal baseSalary,
    decimal allowances,
    decimal insurance,
    decimal tax,
    decimal deductions,
    decimal net,
    string processedBy)
        {
            return new Payslip
            {
                PayrollPeriodId = periodId,
                EmployeeId = emp.EmployeeId,
                WorkingDays = work.ActualWorkingDays,
                PaidLeaveDays = 0,
                BaseSalary = baseSalary,
                TotalAllowances = allowances,
                SocialInsuranceAmount = insurance,
                TaxAmount = tax,
                OtherDeductions = deductions,
                NetSalary = net,
                PaymentDate = DateTime.Now,
                Remarks = work.LateCount > 0
                    ? $"Bị phạt đi muộn {work.LateCount} lần"
                    : "Hợp lệ",
                CreatedBy = processedBy,
                UpdatedBy = processedBy
            };
        }
        private async Task UpdatePeriod(PayrollPeriod period, string user)
        {
            period.Status = Status.PayrollStatus.Processing;
            period.UpdatedBy = user;
            period.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdatePeriodAsync(period);
        }

        public async Task<bool> ApprovePayrollAsync(int periodId, string approvedBy)
        {
            var period = await _repo.GetPeriodByIdAsync(periodId);
            if (period == null || period.Status == Status.PayrollStatus.Draft) return false;

            period.Status = Status.PayrollStatus.Approved;
            period.UpdatedBy = approvedBy;
            period.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdatePeriodAsync(period);
            return true;
        }
    }
}
