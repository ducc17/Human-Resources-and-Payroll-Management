namespace SmartHR_Payroll.ViewModels.Department
{
    // Dành cho thông tin chung của Phòng ban
    public class DepartmentDetailViewModel
    {
        public int DepartmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }

        // Danh sách nhân viên nằm trong phòng ban này
        public List<EmployeeInDepartmentViewModel> Employees { get; set; } = new List<EmployeeInDepartmentViewModel>();
    }

    // Dành cho từng dòng nhân viên hiển thị trong bảng
    public class EmployeeInDepartmentViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PositionName { get; set; } // Chức vụ
    }
}