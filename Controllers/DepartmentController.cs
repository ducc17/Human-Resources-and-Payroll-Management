using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Details(int id)
        {
            // Gọi Service lấy dữ liệu chi tiết
            var data = await _departmentService.GetDepartmentDetailAsync(id);

            // Nếu không tìm thấy phòng ban thì báo lỗi 404
            if (data == null) return NotFound();

            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Create()
        {
            await LoadManagersDropdown(); // Hứng danh sách Manager
            return View(new Department());
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
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
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);
            if (dept == null) return NotFound();

            // LẤY THÊM SỐ LƯỢNG NHÂN VIÊN ĐỂ ĐẨY LÊN GIAO DIỆN CHẶN NÚT KHÓA
            var detail = await _departmentService.GetDepartmentDetailAsync(id);
            ViewBag.EmployeeCount = detail?.Employees.Count ?? 0;

            await LoadManagersDropdown(dept.ManagerId);
            return View(dept);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department model)
        {
            if (id != model.DepartmentId) return BadRequest();

            // CHẶN BẢO MẬT 1: Không cho khóa phòng ban nếu đang có nhân sự
            if (model.IsDeleted)
            {
                var detail = await _departmentService.GetDepartmentDetailAsync(id);
                if (detail != null && detail.Employees.Any())
                {
                    TempData["Error"] = $"Không thể khóa! Phòng ban này đang có {detail.Employees.Count} nhân sự.";
                    await LoadManagersDropdown(model.ManagerId);
                    ViewBag.EmployeeCount = detail.Employees.Count;
                    return View(model);
                }
            }

            // CHẶN BẢO MẬT 2 (XỬ LÝ CASE CỦA BẠN): 
            // Nếu phòng ban đang HOẠT ĐỘNG và có CHỌN MANAGER
            if (!model.IsDeleted && model.ManagerId.HasValue)
            {
                // Kiểm tra xem Manager này có đang ôm phòng ban nào khác đang hoạt động không
                bool isConflict = await _departmentService.CheckManagerConflictAsync(model.ManagerId.Value, id);

                if (isConflict)
                {
                    // Đẩy ngược lại giao diện và báo lỗi
                    TempData["Error"] = "Không thể mở khóa hoặc gán Quản lý này! Nhân sự bạn chọn đang quản lý một phòng ban khác đang hoạt động. Vui lòng để trống Quản lý hoặc chọn người khác.";

                    await LoadManagersDropdown(model.ManagerId);
                    var detail = await _departmentService.GetDepartmentDetailAsync(id);
                    ViewBag.EmployeeCount = detail?.Employees.Count ?? 0;
                    return View(model);
                }
            }

            string currentUserName = User.Identity?.Name ?? "Admin System";

            await _departmentService.UpdateAsync(model, currentUserName);

            TempData["Success"] = "Cập nhật phòng ban thành công!";
            return RedirectToAction(nameof(Index));
        }




        [HttpPost]
        [Authorize(Roles = "HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            // Kiểm tra xem phòng ban có nhân viên nào không
            var deptDetail = await _departmentService.GetDepartmentDetailAsync(id);
            if (deptDetail != null && deptDetail.Employees.Any())
            {
                // Nếu có nhân viên -> Báo lỗi và không cho khóa
                TempData["Error"] = $"Không thể khóa! Phòng ban này đang có {deptDetail.Employees.Count} nhân sự.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu == 0 nhân viên thì mới tiến hành khóa
            await _departmentService.DeactivateAsync(id);
            TempData["Success"] = "Đã khóa phòng ban này!";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadManagersDropdown(int? currentManagerId = null)
        {
            var managers = await _departmentService.GetEmployeesForDropdownAsync(currentManagerId);

            // Format hiển thị ra dropdown: "EMP01 - Nguyễn Văn A"
            var list = managers.Select(e => new {
                Id = e.EmployeeId,
                FullName = $"{e.EmployeeCode} - {e.FirstName} {e.LastName}"
            });

            ViewBag.Employees = new SelectList(list, "Id", "FullName", currentManagerId);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerRequest request)
        {
            try
            {
                // Lấy tên người đang thao tác
                string currentUserName = User.Identity?.Name ?? "Admin";

                // GIAO PHÓ TOÀN BỘ LOGIC CHO SERVICE
                await _departmentService.AssignManagerAsync(request.DepartmentId, request.EmployeeId, currentUserName);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ Service (VD: Lỗi trùng quản lý) và báo ra màn hình
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Class nhỏ xíu dùng để hứng Data JSON từ View gửi lên
        public class AssignManagerRequest
        {
            public int EmployeeId { get; set; }
            public int DepartmentId { get; set; }
        }
    }
}