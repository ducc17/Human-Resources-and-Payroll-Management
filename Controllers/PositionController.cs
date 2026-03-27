using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.Controllers
{
    public class PositionController : Controller
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        private async Task LoadDepartmentsDropdown()
        {
            var depts = await _positionService.GetDepartmentsForDropdownAsync();
            ViewBag.Departments = depts.Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = $"{d.Code} - {d.Name}"
            }).ToList();
        }

        public async Task<IActionResult> Index(int? departmentId, string? sortBy)
        {
            // Lấy dữ liệu đã lọc/sắp xếp
            var data = await _positionService.GetAllWithDetailsAsync(departmentId, sortBy);

            // Lấy danh sách phòng ban cho Dropdown lọc
            var depts = await _positionService.GetDepartmentsForDropdownAsync();
            // Thêm departmentId vào tham số thứ 4 của SelectList
            ViewBag.Departments = new SelectList(depts, "DepartmentId", "Name", departmentId);

            // Lưu lại trạng thái lọc để form không bị reset
            ViewBag.CurrentDept = departmentId;
            ViewBag.CurrentSort = sortBy;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsDropdown();
            return View(new Position());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Position model)
        {
            // Lấy tên user từ HttpContext (Controller có quyền làm việc này)
            string currentUserName = User.Identity?.Name ?? "Admin System";

            // Đẩy Model và Tên User xuống cho Service tự lo logic
            await _positionService.CreateAsync(model, currentUserName);

            TempData["Success"] = $"Thêm vị trí {model.Code} thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pos = await _positionService.GetByIdAsync(id);
            if (pos == null) return NotFound();

            await LoadDepartmentsDropdown();
            return View(pos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Position model)
        {
            if (id != model.PositionId) return BadRequest();

            string currentUserName = User.Identity?.Name ?? "Admin System";

            // Đẩy Model và Tên User xuống cho Service xử lý
            await _positionService.UpdateAsync(model, currentUserName);

            TempData["Success"] = "Cập nhật vị trí thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _positionService.DeactivateAsync(id);
            TempData["Success"] = "Đã khóa vị trí này!";
            return RedirectToAction(nameof(Index));
        }
    }
}