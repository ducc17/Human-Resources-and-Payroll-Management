using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Employee;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var model = await _employeeService.GetEmployeesPagedAsync(page, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new Employee
            {
                HireDate = DateOnly.FromDateTime(DateTime.Today),
                Status = Status.EmployeeStatus.Active,
                Gender = Status.Gender.Male,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today)
            };

            await PopulateSelectionsAsync(model.DepartmentId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {

            ModelState.Remove("DepartmentId");
            ModelState.Remove("PositionId");
            ModelState.Remove("RoleId");

            ModelState.Remove(nameof(model.EmployeeCode));
            ModelState.Remove(nameof(model.FirstName));
            ModelState.Remove(nameof(model.LastName));
            ModelState.Remove(nameof(model.Email));
            ModelState.Remove(nameof(model.PhoneNumber));
            ModelState.Remove(nameof(model.Address)); 
            ModelState.Remove(nameof(model.BankAccountNumber));
            ModelState.Remove(nameof(model.BankName));


            ModelState.Remove(nameof(model.Department));
            ModelState.Remove(nameof(model.Position));
            ModelState.Remove(nameof(model.Role));


            ModelState.Remove(nameof(model.Contracts));
            ModelState.Remove(nameof(model.Attendances));
            ModelState.Remove(nameof(model.LeaveRequests));
            ModelState.Remove(nameof(model.Allowances));
            ModelState.Remove(nameof(model.Deductions));
            ModelState.Remove(nameof(model.Payslips));

            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("UpdatedBy");

            if (!ModelState.IsValid)
            {
                await PopulateSelectionsAsync(model.DepartmentId);
                return View(model);
            }

            var actor = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.Identity?.Name
                        ?? "System";

            var result = await _employeeService.CreateEmployeeAsync(model, actor);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await PopulateSelectionsAsync(model.DepartmentId);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(int employeeId, int page = 1)
        {
            var actor = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.Identity?.Name
                        ?? "System";

            var result = await _employeeService.BanEmployeeAsync(employeeId, actor);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index), new { page });
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index), new { page });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unban(int employeeId, int page = 1)
        {
            var actor = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.Identity?.Name
                        ?? "System";

            var result = await _employeeService.UnbanEmployeeAsync(employeeId, actor);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index), new { page });
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index), new { page });
        }

        [HttpGet]
        public async Task<IActionResult> Contracts(int employeeId)
        {
            var model = await _employeeService.GetEmployeeContractsAsync(employeeId);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        private async Task PopulateSelectionsAsync(int? selectedDepartmentId = null)
        {
            var lookups = await _employeeService.GetCreateEmployeeLookupsAsync();

            ViewBag.Departments = new SelectList(lookups.Departments.Select(d => new
            {
                d.DepartmentId,
                DisplayText = $"{d.Code} - {d.Name}"
            }), "DepartmentId", "DisplayText");

            var filteredPositions = lookups.Positions
                .Where(p => selectedDepartmentId.HasValue && selectedDepartmentId.Value > 0 && p.DepartmentId == selectedDepartmentId.Value)
                .Select(p => new
                {
                    p.PositionId,
                    DisplayText = $"{p.Code} - {p.Name}"
                });

            ViewBag.Positions = new SelectList(filteredPositions, "PositionId", "DisplayText");

            ViewBag.PositionOptions = lookups.Positions.Select(p => new
            {
                p.PositionId,
                p.DepartmentId,
                DisplayText = $"{p.Code} - {p.Name}"
            }).ToList();

            ViewBag.Roles = new SelectList(lookups.Roles, "RoleId", "Name");
        }
    }
}