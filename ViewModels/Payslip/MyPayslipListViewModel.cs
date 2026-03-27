namespace SmartHR_Payroll.ViewModels.Payslip
{
    public class MyPayslipListViewModel
    {
        public int PayslipId { get; set; }
        public string PeriodName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; }
    }

    public class PayslipDetailViewModel
    {
        public int PayslipId { get; set; }
        public string PeriodName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }

        public decimal WorkingDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal BaseSalary { get; set; }

        public decimal TotalAllowances { get; set; }
        public decimal SocialInsuranceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal OtherDeductions { get; set; }

        public decimal GrossSalary => BaseSalary + TotalAllowances;
        public decimal TotalDeductions => SocialInsuranceAmount + TaxAmount + OtherDeductions;
        public decimal NetSalary { get; set; }

        public DateTime? PaymentDate { get; set; }
        public string Remarks { get; set; }

        public List<PayslipAllowanceDetail> AllowanceDetails { get; set; } = new List<PayslipAllowanceDetail>();
        public List<PayslipDeductionDetail> DeductionDetails { get; set; } = new List<PayslipDeductionDetail>();
    }

    public class PayslipAllowanceDetail
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class PayslipDeductionDetail
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
