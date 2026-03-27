using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService) { _roleService = roleService; }

        public async Task<IActionResult> Index(string? sortBy)
        {
            var data = await _roleService.GetAllAsync(sortBy);
            ViewBag.CurrentSort = sortBy;
            return View(data);
        }

        [HttpGet]
        public IActionResult Create() => View(new Role());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role model)
        {
            string currentUserName = User.Identity?.Name ?? "Admin System";
            await _roleService.CreateAsync(model, currentUserName);
            TempData["Success"] = "Thêm vai trò thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role model)
        {
            if (id != model.RoleId) return BadRequest();

            string currentUserName = User.Identity?.Name ?? "Admin System";
            await _roleService.UpdateAsync(model, currentUserName);
            TempData["Success"] = "Cập nhật vai trò thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _roleService.DeactivateAsync(id);
            TempData["Success"] = "Đã khóa vai trò này!";
            return RedirectToAction(nameof(Index));
        }
    }
}