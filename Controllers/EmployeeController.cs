using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Employee;
using System.Security.Claims;


namespace SmartHR_Payroll.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Index(
                int page = 1,
                string keyword = "",
                string status = ""
            )
        {
            const int pageSize = 10;
            int? departmentId = null;

            // Manager chỉ xem phòng của mình
            if (User.IsInRole("Manager"))
            {
                var claim = User.FindFirst("DepartmentId");

                if (claim != null && int.TryParse(claim.Value, out int deptId))
                {
                    departmentId = deptId;
                }
            }

            var model = await _employeeService.GetEmployeesPagedAsync(
                page,
                pageSize,
                departmentId,
                keyword,
                status
            );

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create()
        {
            var model = new Employee
            {
                HireDate = DateOnly.FromDateTime(DateTime.Today),
                Status = Status.EmployeeStatus.Active,
                Gender = Status.Gender.Male,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today)
            };

            await PopulateSelectionsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(Employee model)
        {

            ModelState.Remove("RoleId");

            ModelState.Remove(nameof(model.EmployeeCode));
            ModelState.Remove(nameof(model.FirstName));
            ModelState.Remove(nameof(model.LastName));
            ModelState.Remove(nameof(model.Email));
            ModelState.Remove(nameof(model.PhoneNumber));
            ModelState.Remove(nameof(model.Address)); 
            ModelState.Remove(nameof(model.BankAccountNumber));
            ModelState.Remove(nameof(model.Bank));
            ModelState.Remove(nameof(model.Job));
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
                await PopulateSelectionsAsync();
                return View(model);
            }

            var actor = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.Identity?.Name
                        ?? "System";

            var result = await _employeeService.CreateEmployeeAsync(model, actor);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await PopulateSelectionsAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
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
        [Authorize(Roles = "Admin,HR")]
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
        [Authorize(Roles = "Admin,HR,Manager")]
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

        [Authorize(Roles = "Admin,HR")]
        private async Task PopulateSelectionsAsync()
        {
            var lookups = await _employeeService.GetCreateEmployeeLookupsAsync();

            ViewBag.Jobs = new SelectList(lookups.Jobs.Select(j => new
            {
                j.JobId,
                DisplayText = $"{j.Department.Code} - {j.Department.Name} | {j.Position.Code} - {j.Position.Name}"
            }), "JobId", "DisplayText");

            ViewBag.Banks = new SelectList(lookups.Banks.Select(b => new
            {
                b.BankId,
                DisplayText = $"{b.ShortName} - {b.BankName}"
            }), "BankId", "DisplayText");

            ViewBag.Roles = new SelectList(lookups.Roles, "RoleId", "Name");
        }
    }
}