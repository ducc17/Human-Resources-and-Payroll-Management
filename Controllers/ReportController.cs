using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    [Authorize(Roles = "Admin,HR,Manager")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> TeamReport(DateOnly? fromDate, DateOnly? toDate)
        {
            var from = fromDate ?? DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
            var to = toDate ?? DateOnly.FromDateTime(DateTime.Now);

            int? departmentId = null;

            if (User.IsInRole("Manager"))
            {
                var claim = User.FindFirst("DepartmentId");

                if (claim != null && int.TryParse(claim.Value, out int deptId))
                {
                    departmentId = deptId;
                }
            }

            var model = await _reportService.GetTeamReportAsync(from, to, departmentId);

            ViewBag.FromDate = from.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to.ToString("yyyy-MM-dd");

            return View(model);
        }
    }
}