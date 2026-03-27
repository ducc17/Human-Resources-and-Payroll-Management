using System.ComponentModel.DataAnnotations;

namespace SmartHR_Payroll.ViewModels.Insurance
{
    public class InsuranceViewModel
    {
        public int InsuranceId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mã bảo hiểm")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Tên bảo hiểm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tỷ lệ NV đóng")]
        [Range(0, 100, ErrorMessage = "Tỷ lệ từ 0 - 100%")]
        public decimal EmployeeRate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tỷ lệ CTY đóng")]
        [Range(0, 100, ErrorMessage = "Tỷ lệ từ 0 - 100%")]
        public decimal CompanyRate { get; set; }

        public decimal? MaxSalaryLimit { get; set; }
    }
}
