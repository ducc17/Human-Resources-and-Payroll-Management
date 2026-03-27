using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Contract;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(int? employeeId)
        {
            var model = new CreateContractViewModel
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                IsActive = false
            };

            await PopulateEmployeesAsync();

            if (employeeId.HasValue && employeeId.Value > 0)
            {
                model.EmployeeId = employeeId.Value;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(CreateContractViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEmployeesAsync();
                return View(model);
            }

            var actor = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.Identity?.Name
                        ?? "System";

            var result = await _contractService.CreateContractAsync(model, actor);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await PopulateEmployeesAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Cancel(int contractId, int employeeId)
        {
            var actor = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.Identity?.Name
                        ?? "System";

            var result = await _contractService.CancelContractAsync(contractId, actor);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Contracts", "Employee", new { employeeId });
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Contracts", "Employee", new { employeeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Confirm(int contractId, int employeeId)
        {
            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdClaim, out var currentUserId) || currentUserId != employeeId)
            {
                TempData["ErrorMessage"] = "Không có quyền xác nhận hợp đồng này.";
                return RedirectToAction("Contracts", "Employee", new { employeeId });
            }

            var actor = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.Identity?.Name
                        ?? "System";

            var result = await _contractService.ConfirmContractByEmployeeAsync(contractId, employeeId, actor);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Contracts", "Employee", new { employeeId });
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Contracts", "Employee", new { employeeId });
        }

        private async Task PopulateEmployeesAsync()
        {
            var employees = await _contractService.GetEmployeesForCreateAsync();
            ViewBag.Employees = new SelectList(employees.Select(e => new
            {
                e.EmployeeId,
                DisplayText = e.EmployeeCode
            }), "EmployeeId", "DisplayText");
        }
    }
}
