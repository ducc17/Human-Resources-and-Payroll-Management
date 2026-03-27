using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Insurance;

namespace SmartHR_Payroll.Controllers
{
    [Authorize(Roles ="Admin,Manager")]
    public class InsuranceController : Controller
    {
        private readonly IInsuranceService _insuranceService;
        public InsuranceController(IInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _insuranceService.GetAllInsurancesAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InsuranceViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = await _insuranceService.CreateInsuranceAsync(model, User.Identity.Name ?? "System");
                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm mới bảo hiểm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("Code", "Mã bảo hiểm này đã tồn tại trong hệ thống!");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _insuranceService.GetInsuranceByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InsuranceViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = await _insuranceService.UpdateInsuranceAsync(model, User.Identity.Name ?? "System");
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật bảo hiểm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("Code", "Mã bảo hiểm này đã tồn tại trong hệ thống!");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _insuranceService.DeleteInsuranceAsync(id, User.Identity.Name ?? "System");
            TempData["SuccessMessage"] = "Đã xóa bảo hiểm thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
