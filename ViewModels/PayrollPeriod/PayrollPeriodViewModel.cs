using SmartHR_Payroll.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartHR_Payroll.ViewModels.PayrollPeriod
{
    public class PayrollPeriodViewModel
    {
        public int PayrollPeriodId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên kỳ lương")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateOnly EndDate { get; set; }

        public Status.PayrollStatus Status { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalNetSalary { get; set; }
    }
}
