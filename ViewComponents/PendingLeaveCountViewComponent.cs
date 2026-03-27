using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;

namespace SmartHR_Payroll.ViewComponents
{
    public class PendingLeaveCountViewComponent : ViewComponent
    {
        private readonly ILeaveRequestService _service;

        public PendingLeaveCountViewComponent(ILeaveRequestService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = await _service.CountPendingAsync(UserClaimsPrincipal);
            return View(count);
        }
    }
}