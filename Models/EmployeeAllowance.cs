using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class EmployeeAllowance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int AllowanceId { get; set; }
        public virtual Allowance Allowance { get; set; }
        public decimal Amount { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }
}
