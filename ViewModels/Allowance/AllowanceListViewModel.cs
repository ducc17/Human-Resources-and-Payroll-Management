namespace SmartHR_Payroll.ViewModels.Allowance
{
    public class AllowanceListViewModel
    {
        public int AllowanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsTaxable { get; set; }
        public int EmployeeCount { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AllowanceIndexViewModel
    {
        public List<AllowanceListViewModel> Allowances { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
