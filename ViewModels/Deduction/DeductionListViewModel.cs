namespace SmartHR_Payroll.ViewModels.Deduction
{
    public class DeductionListViewModel
    {
        public int DeductionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DeductionIndexViewModel
    {
        public List<DeductionListViewModel> Deductions { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
