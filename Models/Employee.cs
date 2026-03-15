using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.Models
{
    public class Employee : AuditableEntity
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public DateOnly HireDate { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<EmployeeAllowance> Allowances { get; set; } = new List<EmployeeAllowance>();
        public virtual ICollection<EmployeeDeduction> Deductions { get; set; } = new List<EmployeeDeduction>();
        public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();

    }
}
