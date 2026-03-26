namespace SmartHR_Payroll.ViewModels.Position
{
    public class PositionListViewModel
    {
        public int PositionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; } // Tên phòng ban
        public int EmployeeCount { get; set; } // Số lượng nhân viên đang giữ vị trí này
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}