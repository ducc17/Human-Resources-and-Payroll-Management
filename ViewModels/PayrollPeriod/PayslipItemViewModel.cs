namespace SmartHR_Payroll.ViewModels.PayrollPeriod
{
    public class PayslipItemViewModel
    {
        public int PayslipId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public decimal WorkingDays { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal SocialInsuranceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal TotalDeductions => SocialInsuranceAmount + TaxAmount + OtherDeductions;
        public decimal NetSalary { get; set; }
        public List<AllowanceDetailViewModel> AllowanceDetails { get; set; } = new List<AllowanceDetailViewModel>();
        public List<DeductionDetailViewModel> DeductionDetails { get; set; } = new List<DeductionDetailViewModel>();
    }

    public class AllowanceDetailViewModel
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class DeductionDetailViewModel
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
