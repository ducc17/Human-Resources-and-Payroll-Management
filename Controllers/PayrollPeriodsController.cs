using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.PayrollPeriod;

namespace SmartHR_Payroll.Controllers
{

    [Authorize(Roles = "Admin,Manager")]
    public class PayrollPeriodController : Controller
    {
        private readonly IPayrollPeriodService _service;

        public PayrollPeriodController(IPayrollPeriodService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var periods = await _service.GetAllPeriodsAsync();
            return View(periods);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _service.CreatePeriodAsync(model, User.Identity.Name ?? "System");
                TempData["SuccessMessage"] = "Đã tạo kỳ lương mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> PayrollSheet(int id)
        {
            var sheet = await _service.GetPayrollSheetAsync(id);
            if (sheet == null) return NotFound();
            return View(sheet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int id)
        {
            bool success = await _service.ProcessPayrollAsync(id, User.Identity.Name ?? "System");
            if (success)
                TempData["SuccessMessage"] = "Đã quét và tính toán dữ liệu lương thành công!";
            else
                TempData["ErrorMessage"] = "Không thể chạy lương (Kỳ này có thể đã được duyệt).";

            return RedirectToAction(nameof(PayrollSheet), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            bool success = await _service.ApprovePayrollAsync(id, User.Identity.Name ?? "System");
            if (success)
                TempData["SuccessMessage"] = "Kỳ lương đã được chốt và duyệt chính thức!";
            else
                TempData["ErrorMessage"] = "Lỗi: Không thể duyệt kỳ lương này.";

            return RedirectToAction(nameof(PayrollSheet), new { id = id });
        }
    }
}
