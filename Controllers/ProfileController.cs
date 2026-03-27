using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Profile;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public ProfileController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Hiển thị thông tin Profile
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1. Lấy ID người dùng đang đăng nhập
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Auth");

            int employeeId = int.Parse(userIdStr);

            // 2. Gọi hàm GetByIdAsync (đã có sẵn Include Department và Position)
            var emp = await _employeeService.GetByIdAsync(employeeId);
            if (emp == null) return NotFound();

            // 3. Đổ dữ liệu TƯƠI từ DB vào ViewModel
            var model = new ProfileViewModel
            {
                EmployeeCode = emp.EmployeeCode,
                FullName = emp.FirstName + " " + emp.LastName,

                // Đây là 2 dòng quan trọng nhất để fix lỗi hiển thị sai:
                PositionName = emp.Job.Position?.Name ?? "Chưa cập nhật",
                DepartmentName = emp.Job.Department?.Name ?? "Chưa cập nhật",

                HireDate = emp.HireDate,
                Email = emp.Email,
                PhoneNumber = emp.PhoneNumber ?? "Chưa cập nhật",
                Address = emp.Address ?? "Chưa cập nhật",
                BankName = emp.Bank.BankName,
                BankAccountNumber = emp.BankAccountNumber
            };

            return View(model);
        }

        // Hiển thị Form chỉnh sửa (GET)
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            var editModel = await _employeeService.GetEditProfileAsync(int.Parse(userId));
            if (editModel == null) return NotFound();

            return View(editModel);
        }

        // Xử lý lưu thay đổi (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            // 1. Kiểm tra tính hợp lệ của dữ liệu đầu vào (Validation)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 2. Lấy ID người dùng đang đăng nhập từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            int currentUserId = int.Parse(userId);

            // 3. Bảo mật: Đảm bảo người dùng chỉ sửa được profile của chính mình
            if (currentUserId != model.EmployeeId)
            {
                return Forbid();
            }

            // 4. Gọi Service để cập nhật dữ liệu vào DB
            var result = await _employeeService.UpdateProfileAsync(model);

            if (result.Success)
            {
                // Lưu thông báo thành công để hiển thị ở trang Index
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi từ phía nghiệp vụ (Service), hiển thị lỗi lên Form
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }
    }
}