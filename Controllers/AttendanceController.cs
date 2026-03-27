using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services;
using SmartHR_Payroll.Services.IServices;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;
        public AttendanceController(IAttendanceService attendanceService, IEmployeeService employeeService)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
        }


        [HttpGet]
        [Authorize(Roles = "Manager, HR")]
        public async Task<IActionResult> Index(string? search, int? departmentId, DateOnly? fromDate, DateOnly? toDate, string? status, int page = 1)
        {
            int pageSize = 10;

            // 1. Gọi Service (Giao phó toàn bộ việc lọc, gom nhóm và phân trang cho Service)
            var result = await _attendanceService.GetAllAttendancesAsync(search, fromDate, toDate, status, departmentId, page, pageSize);

            // 2. Nhận kết quả và đẩy ra ViewBags để View hiển thị thanh phân trang & giữ bộ lọc
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.TotalPages; // Lấy TotalPages từ Service tính sẵn

            ViewBag.Search = search;
            ViewBag.DepartmentId = departmentId;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            ViewBag.Departments = await _attendanceService.GetAllDepartmentsAsync();

            // 3. Điều hướng trả về Data đã được cắt gọn
            return View(result.Items);
        }

        // Trang hiển thị Form Import
        [HttpGet]
        [Authorize(Roles = "Manager, HR")]
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
        [Authorize]
        public async Task<IActionResult> MyHistory(int? month, int? year)
        {
            // 1. Lấy ID nhân viên
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Auth");
            int empId = int.Parse(userIdStr);

            // 2. Xác định tháng/năm (mặc định là hiện tại nếu không chọn)
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            // 3. ĐẨY HẾT LOGIC CHO SERVICE
            var dailyData = await _attendanceService.GetMyAttendanceCalendarAsync(empId, currentMonth, currentYear);

            // 4. Trả View
            ViewBag.Month = currentMonth;
            ViewBag.Year = currentYear;

            return View(dailyData);
        }
    }
}