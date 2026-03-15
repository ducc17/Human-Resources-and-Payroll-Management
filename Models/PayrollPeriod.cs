using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.Models
{
    public class PayrollPeriod : AuditableEntity
    {
        public int PayrollPeriodId { get; set; }
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;
        public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
    }
}
