using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services;
using SmartHR_Payroll.Services.IServices.SmartHR_Payroll.Services.IServices;


namespace SmartHR_Payroll.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AttendanceService _service;

        public AttendanceController(AttendanceService service) => _service = service;

        public async Task<IActionResult> Index()
        {
            int empId = 2; // Giả sử ID nhân viên là 1

            // Lấy dữ liệu từ Service
            var todayAtt = await _service.GetTodayAttendanceAsync(empId);
            var history = await _service.GetHistoryAsync(empId);

            // Truyền trực tiếp qua ViewBag
            ViewBag.History = history;

            // Truyền TodayAttendance làm Model chính của View
            return View(todayAtt);
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn()
        {
            try
            {
                int empId = 1; // Tạm thời hardcode, sau này lấy từ User.Identity
                await _service.CheckInAsync(empId);
                TempData["Success"] = "Check-in thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut()
        {
            try
            {
                int empId = 1;
                await _service.CheckOutAsync(empId);
                TempData["Success"] = "Check-out thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
