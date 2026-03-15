using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class Deduction : AuditableEntity
    {
        public int DeductionId { get; set; }
        public string Name { get; set; }
    }
}
