using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.Models
{
    public class LeaveRequest : AuditableEntity
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Reason { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public int? ApprovedById { get; set; }
    }
}
