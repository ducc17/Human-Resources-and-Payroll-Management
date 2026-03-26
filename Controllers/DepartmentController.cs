using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // 1. Hàm hỗ trợ tạo Dropdown Nhân viên
        private async Task LoadEmployeesDropdown(int? currentManagerId = null)
        {
            var emps = await _departmentService.GetEmployeesForDropdownAsync(currentManagerId);
            ViewBag.Employees = emps.Select(e => new SelectListItem
            {
                Value = e.EmployeeId.ToString(),
                Text = $"{e.EmployeeCode} - {e.FirstName} {e.LastName}"
            }).ToList();
        }



        public async Task<IActionResult> Index(string? sortBy)
        {
            var data = await _departmentService.GetAllWithDetailsAsync(sortBy);
            ViewBag.CurrentSort = sortBy;
            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Gọi Service lấy dữ liệu chi tiết
            var data = await _departmentService.GetDepartmentDetailAsync(id);

            // Nếu không tìm thấy phòng ban thì báo lỗi 404
            if (data == null) return NotFound();

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadEmployeesDropdown();
            return View(new Department());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            // Lấy tên người dùng hiện tại
            string currentUserName = User.Identity?.Name ?? "Admin System";

            // Truyền xuống cho Service lo liệu
            await _departmentService.CreateAsync(department, currentUserName);

            TempData["Success"] = $"Thêm phòng ban thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);
            if (dept == null) return NotFound();

            // Truyền ManagerId hiện tại vào để nó luôn hiện trong Dropdown
            await LoadEmployeesDropdown(dept.ManagerId);
            return View(dept);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department model)
        {
            if (id != model.DepartmentId) return BadRequest();

            string currentUserName = User.Identity?.Name ?? "Admin System";

            // Truyền xuống cho Service lo liệu
            await _departmentService.UpdateAsync(model, currentUserName);

            TempData["Success"] = "Cập nhật phòng ban thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _departmentService.DeactivateAsync(id);
            TempData["Success"] = "Đã khóa hoạt động phòng ban này!";
            return RedirectToAction(nameof(Index));
        }
    }
}