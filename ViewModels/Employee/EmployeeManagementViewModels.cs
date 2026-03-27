using System.ComponentModel.DataAnnotations;
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
        public string RoleName { get; set; } = string.Empty;
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
        public bool IsDeleted { get; set; }
    }

    public class EmployeeContractsViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<EmployeeContractItemViewModel> Contracts { get; set; } = new();
    }

    public class AddEmployeeAllowanceViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn khoản phụ cấp.")]
        public int AllowanceId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền phụ cấp.")]
        [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "Số tiền phụ cấp phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày áp dụng.")]
        public DateOnly EffectiveDate { get; set; }

    }

    public class EmployeeAllowanceItemViewModel
    {
        public string AllowanceName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }

    public class EmployeeAllowanceListViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public string Keyword { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<EmployeeAllowanceItemViewModel> Items { get; set; } = new();
    }

    public class AddEmployeeDeductionViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn khoản trừ.")]
        public int DeductionId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền khoản trừ.")]
        [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "Số tiền khoản trừ phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày áp dụng.")]
        public DateOnly EffectiveDate { get; set; }
    }

    public class EmployeeDeductionItemViewModel
    {
        public string DeductionName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }

    public class EmployeeDeductionListViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public string Keyword { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<EmployeeDeductionItemViewModel> Items { get; set; } = new();
    }
}
