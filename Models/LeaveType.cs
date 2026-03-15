using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHR_Payroll.Models
{
    public class LeaveType : AuditableEntity
    {
        public int LeaveTypeId { get; set; }
        public string Name { get; set; }
        public int DefaultDaysPerYear { get; set; }
        public bool IsPaidLeave { get; set; }
    }
}
