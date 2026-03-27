namespace SmartHR_Payroll.Models
{
    public class Job : AuditableEntity
    {
        public int JobId { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
