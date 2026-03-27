using System.ComponentModel.DataAnnotations;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.ViewModels.Contract
{
    public class CreateContractViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Mã hợp đồng không được để trống")]
        [StringLength(50, ErrorMessage = "Mã hợp đồng tối đa 50 ký tự")]
        public string ContractNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại hợp đồng")]
        public ContractType Type { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Range(1, 999999999999.99, ErrorMessage = "Lương cơ bản phải lớn hơn 0")]
        public decimal BaseSalary { get; set; }

        public bool IsActive { get; set; } = false;
    }
}
