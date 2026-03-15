using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class EmployeeDeduction
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int DeductionId { get; set; }
        public virtual Deduction Deduction { get; set; }
        public decimal Amount { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }
}
