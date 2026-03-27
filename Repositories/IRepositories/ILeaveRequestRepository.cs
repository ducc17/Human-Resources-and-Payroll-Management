using SmartHR_Payroll.Models;

public interface ILeaveRequestRepository
{
    Task<List<LeaveRequest>> GetAllAsync(
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? status,
        int? departmentId,
        int? employeeId
    );

    Task<LeaveRequest?> GetByIdAsync(int id);

    Task AddAsync(LeaveRequest entity);

    Task UpdateAsync(LeaveRequest entity);

    Task SaveChangesAsync();

    Task<int> CountPendingAsync(int? departmentId, int? employeeId);
}