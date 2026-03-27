namespace SmartHR_Payroll.ViewModels.Role
{
    public class RoleListViewModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EmployeeCount { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}