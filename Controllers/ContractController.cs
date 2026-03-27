using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Contract;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateContractViewModel
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                IsActive = false
            };

            await PopulateEmployeesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
