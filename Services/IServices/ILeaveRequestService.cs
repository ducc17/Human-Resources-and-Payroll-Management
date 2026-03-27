using SmartHR_Payroll.Models;
using System.Security.Claims;

public interface ILeaveRequestService
{
    Task<List<LeaveRequest>> GetAllAsync(
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? status,
        ClaimsPrincipal user
    );

    Task CreateAsync(LeaveRequest model, ClaimsPrincipal user);

    Task ApproveAsync(int id, ClaimsPrincipal user);

    Task RejectAsync(int id, ClaimsPrincipal user);
}