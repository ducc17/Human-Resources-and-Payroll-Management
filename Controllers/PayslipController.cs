using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Payslip;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    [Authorize] 
    public class PayslipController : Controller
    {
        private readonly IPayslipService _service;

        public PayslipController(IPayslipService service)
        {
            _service = service;
        }

        public async Task<IActionResult> MyPayslips()
        {
            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int employeeId) || employeeId == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ nhân viên của tài khoản này.";
                return View(new List<MyPayslipListViewModel>());
            }

            var list = await _service.GetMyPayslipsAsync(employeeId);
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var detail = await _service.GetPayslipDetailAsync(id);
            if (detail == null) return NotFound();

            return View(detail);
        }

        public async Task<IActionResult> Print(int id)
        {
            var detail = await _service.GetPayslipDetailAsync(id);
            if (detail == null) return NotFound();

            return View("Print", detail);
        }
    }
}
