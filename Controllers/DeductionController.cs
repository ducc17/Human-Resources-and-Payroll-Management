using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.Controllers
{
    public class DeductionController : Controller
    {
        private readonly IDeductionService _deductionService;

        public DeductionController(IDeductionService deductionService)
        {
            _deductionService = deductionService;
        }

        public async Task<IActionResult> Index(string? keyword, string? sortBy, int page = 1)
        {
            var data = await _deductionService.GetAllAsync(keyword, sortBy, page, 10);
            ViewBag.CurrentKeyword = keyword;
            ViewBag.CurrentSort = sortBy;
            return View(data);
        }

        [HttpGet]
        public IActionResult Create() => View(new Deduction());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Deduction model)
        {
            model.Name = string.Join(" ", (model.Name ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Vui lòng nhập tên khấu trừ.");
                return View(model);
            }

            if (await _deductionService.NameExistsAsync(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Tên khấu trừ đã tồn tại. Vui lòng nhập tên khác.");
                return View(model);
            }

            string currentUserName = User.Identity?.Name ?? "Admin System";
            await _deductionService.CreateAsync(model, currentUserName);

            TempData["Success"] = "Thêm khấu trừ thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var deduction = await _deductionService.GetByIdAsync(id);
            if (deduction == null) return NotFound();

            return View(deduction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Deduction model)
        {
            if (id != model.DeductionId) return BadRequest();

            model.Name = string.Join(" ", (model.Name ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Vui lòng nhập tên khấu trừ.");
                return View(model);
            }

            if (await _deductionService.NameExistsAsync(model.Name, model.DeductionId))
            {
                ModelState.AddModelError(nameof(model.Name), "Tên khấu trừ đã tồn tại. Vui lòng nhập tên khác.");
                return View(model);
            }

            string currentUserName = User.Identity?.Name ?? "Admin System";
            await _deductionService.UpdateAsync(model, currentUserName);

            TempData["Success"] = "Cập nhật khấu trừ thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _deductionService.DeactivateAsync(id);
            TempData["Success"] = "Đã khóa khấu trừ này!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            await _deductionService.RestoreAsync(id);
            TempData["Success"] = "Đã khôi phục khấu trừ!";
            return RedirectToAction(nameof(Index));
        }
    }
}
