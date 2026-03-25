using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // Trang chủ của Attendance (Khắc phục lỗi 404 khi vào /Attendance)
        public IActionResult Index()
        {
            return View();
            // Lưu ý: Nhớ giữ lại file Views/Attendance/Index.cshtml của bạn nhé
        }

        // Trang hiển thị Form Import
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        // Xử lý khi bấm nút Upload File Excel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn một file Excel hợp lệ.";
                return View();
            }

            var result = await _attendanceService.ImportExcelAsync(fileExcel);

            TempData["SuccessMessage"] = $"Import thành công {result.SuccessCount} bản ghi.";
            if (result.ErrorCount > 0)
            {
                TempData["WarningMessage"] = $"Có {result.ErrorCount} bản ghi bị lỗi.";
                ViewBag.ErrorList = result.ErrorMessages;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyHistory()
        {
            // 1. Rút "EmployeeId" từ trong vé đăng nhập (Claims) ra
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // Nếu chưa đăng nhập hoặc mất Session -> Đuổi về trang Login
                return RedirectToAction("Login", "Auth");
            }

            // 2. Chuyển ID từ chuỗi (string) sang số (int)
            int employeeId = int.Parse(userIdClaim);

            // 3. Gọi Service truyền đúng số ID đó vào
            var model = await _attendanceService.GetMyAttendanceHistoryAsync(employeeId);

            // 4. Trả dữ liệu ra View
            return View(model);
        }
    }
}