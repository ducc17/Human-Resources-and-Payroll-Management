using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestService _service;

        public LeaveRequestController(ILeaveRequestService service)
        {
            _service = service;
        }

        // ===================== LIST =====================
        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            DateOnly? fromDate,
            DateOnly? toDate,
            string? status)
        {
            var data = await _service.GetAllAsync(
                search,
                fromDate,
                toDate,
                status,
                User
            );

            return View(data);
        }

        // ===================== CREATE =====================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _service.CreateAsync(model, User);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // ===================== APPROVE =====================
        [HttpPost]
        [Authorize(Roles = "Manager,HR")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _service.ApproveAsync(id, User);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ===================== REJECT =====================
        [HttpPost]
        [Authorize(Roles = "Manager,HR")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                await _service.RejectAsync(id, User);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}