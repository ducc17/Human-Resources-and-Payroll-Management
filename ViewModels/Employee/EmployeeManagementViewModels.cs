using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.ViewModels.Employee
{
    public class EmployeeListItemViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public DateOnly HireDate { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public EmployeeStatus Status { get; set; }
    }

    public class EmployeeListViewModel
    {
        public List<EmployeeListItemViewModel> Employees { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class EmployeeContractItemViewModel
    {
        public int ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public ContractType Type { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeContractsViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<EmployeeContractItemViewModel> Contracts { get; set; } = new();
    }
}
