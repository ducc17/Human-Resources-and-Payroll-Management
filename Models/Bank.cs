namespace SmartHR_Payroll.Models
{

    public class Bank : AuditableEntity
    {
        public int BankId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string ShortName { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
