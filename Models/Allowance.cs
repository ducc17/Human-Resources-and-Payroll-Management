using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class Allowance  : AuditableEntity
    {
        public int AllowanceId { get; set; }
        public string Name { get; set; }
        public bool IsTaxable { get; set; }
    }
}
