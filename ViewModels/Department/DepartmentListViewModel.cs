namespace SmartHR_Payroll.ViewModels.Department
{
    public class DepartmentListViewModel
    {
        public int DepartmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int EmployeeCount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin Quản lý
        public string ManagerCode { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
    }
}