using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.TaxBracket;

namespace SmartHR_Payroll.Controllers
{
    [Authorize(Roles ="Admin,Manager")]
    public class TaxBracketController : Controller
    {
        private readonly ITaxService _taxService;
        public TaxBracketController(ITaxService taxService)
        {
            _taxService = taxService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _taxService.GetAllTaxBracketsAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaxBracketViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _taxService.CreateTaxBracketAsync(model, User.Identity.Name ?? "System");
                TempData["SuccessMessage"] = "Thêm bậc thuế thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _taxService.GetTaxBracketByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaxBracketViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _taxService.UpdateTaxBracketAsync(model, User.Identity.Name ?? "System");
                TempData["SuccessMessage"] = "Cập nhật bậc thuế thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

    }
}
