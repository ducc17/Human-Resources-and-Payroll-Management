using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.ViewModels.PayrollPeriod
{
    public class PayrollSheetViewModel
    {
        public PayrollPeriodViewModel Period { get; set; }
        public List<PayslipItemViewModel> Payslips { get; set; } = new List<PayslipItemViewModel>();
    }
}
