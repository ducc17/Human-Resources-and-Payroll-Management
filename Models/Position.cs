using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class Position : AuditableEntity
    {
        public int PositionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    }
}
