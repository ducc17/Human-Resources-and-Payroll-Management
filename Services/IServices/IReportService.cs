using SmartHR_Payroll.ViewModels.Report;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IReportService
    {
        Task<TeamReportViewModel> GetTeamReportAsync(
            DateOnly fromDate,
            DateOnly toDate,
            int? departmentId
        );
    }

}
