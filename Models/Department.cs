using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class Department : AuditableEntity
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ManagerId { get; set; }

        public virtual Employee Manager { get; set; }
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    }
}
