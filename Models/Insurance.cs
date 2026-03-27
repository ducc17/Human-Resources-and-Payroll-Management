namespace SmartHR_Payroll.Models
{
    public class Insurance : AuditableEntity
    {
        public int InsuranceId { get; set; }
        public string Code { get; set; } 
        public string Name { get; set; }

        public decimal EmployeeRate { get; set; }

        public decimal CompanyRate { get; set; }

        public decimal? MaxSalaryLimit { get; set; }
    }
}
