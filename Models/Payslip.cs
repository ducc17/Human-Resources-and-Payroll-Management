using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class Payslip : AuditableEntity
    {
        public int PayslipId { get; set; }
        public int PayrollPeriodId { get; set; }
        public virtual PayrollPeriod PayrollPeriod { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public decimal WorkingDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal SocialInsuranceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalDeductions => SocialInsuranceAmount + TaxAmount + OtherDeductions;
        public decimal NetSalary { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Remarks { get; set; }
    }
}
