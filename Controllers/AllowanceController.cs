using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.Controllers
{
    public class AllowanceController : Controller
    {
        private readonly IAllowanceService _allowanceService;

        public AllowanceController(IAllowanceService allowanceService)
        {
            _allowanceService = allowanceService;
        }

        public async Task<IActionResult> Index(string? keyword, bool? isTaxable, string? sortBy, int page = 1)
        {
            var data = await _allowanceService.GetAllAsync(keyword, isTaxable, sortBy, page, 10);
            ViewBag.CurrentKeyword = keyword;
            ViewBag.CurrentIsTaxable = isTaxable;
            ViewBag.CurrentSort = sortBy;
            return View(data);
        }

        [HttpGet]
        public IActionResult Create() => View(new Allowance());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Allowance model)
        {
            model.Name = string.Join(" ", (model.Name ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Vui lòng nhập tên phụ cấp.");
                return View(model);
            }

            if (await _allowanceService.NameExistsAsync(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Tên phụ cấp đã tồn tại. Vui lòng nhập tên khác.");
                return View(model);
            }

            string currentUserName = User.Identity?.Name ?? "Admin System";
            await _allowanceService.CreateAsync(model, currentUserName);

            TempData["Success"] = "Thêm phụ cấp thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var allowance = await _allowanceService.GetByIdAsync(id);
            if (allowance == null) return NotFound();

            return View(allowance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Allowance model)
        {
            if (id != model.AllowanceId) return BadRequest();

            model.Name = string.Join(" ", (model.Name ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Vui lòng nhập tên phụ cấp.");
                return View(model);
            }

            if (await _allowanceService.NameExistsAsync(model.Name, model.AllowanceId))
            {
                ModelState.AddModelError(nameof(model.Name), "Tên phụ cấp đã tồn tại. Vui lòng nhập tên khác.");
                return View(model);
            }

            string currentUserName = User.Identity?.Name ?? "Admin System";
            await _allowanceService.UpdateAsync(model, currentUserName);

            TempData["Success"] = "Cập nhật phụ cấp thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _allowanceService.DeactivateAsync(id);
            TempData["Success"] = "Đã khóa phụ cấp này!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            await _allowanceService.RestoreAsync(id);
            TempData["Success"] = "Đã khôi phục phụ cấp!";
            return RedirectToAction(nameof(Index));
        }
    }
}
