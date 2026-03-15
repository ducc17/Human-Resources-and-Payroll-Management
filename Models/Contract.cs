using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.Models
{
    public class Contract : AuditableEntity
    {
        public int ContractId { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public string ContractNumber { get; set; }
        public ContractType Type { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
