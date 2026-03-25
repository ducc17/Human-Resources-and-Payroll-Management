namespace SmartHR_Payroll.Models
{
    public class Role : AuditableEntity
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
