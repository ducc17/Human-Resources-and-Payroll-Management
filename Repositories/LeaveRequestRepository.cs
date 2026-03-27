using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly DBCodeFirstContext _context;

        public LeaveRequestRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveRequest>> GetAllAsync(
            string? search,
            DateOnly? fromDate,
            DateOnly? toDate,
            string? status,
            int? departmentId,
            int? employeeId)
        {
            var query = _context.LeaveRequests
                .Include(x => x.Employee)
                    .ThenInclude(e => e.Job)
                        .ThenInclude(j => j.Department)
                .AsQueryable();

            // 1. SEARCH (Mã NV / Tên)
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();

                query = query.Where(x =>
                    (x.Employee.EmployeeCode != null && x.Employee.EmployeeCode.ToLower().Contains(search)) ||
                    (x.Employee.FirstName != null && x.Employee.FirstName.ToLower().Contains(search)) ||
                    (x.Employee.LastName != null && x.Employee.LastName.ToLower().Contains(search))
                );
            }

            // FROM DATE
            if (fromDate.HasValue)
            {
                query = query.Where(x => x.StartDate >= fromDate.Value);
            }

            // TO DATE
            if (toDate.HasValue)
            {
                query = query.Where(x => x.EndDate <= toDate.Value);
            }

            // 4. STATUS
            if (!string.IsNullOrEmpty(status) &&
                Enum.TryParse<LeaveStatus>(status, out var parsedStatus))
            {
                query = query.Where(x => x.Status == parsedStatus);
            }

            // 5. DEPARTMENT (Manager)
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(x => x.Employee.Job.DepartmentId == departmentId.Value);
            }

            // 6. EMPLOYEE (Employee chỉ thấy của mình)
            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.LeaveRequestId == id);
        }

        public async Task AddAsync(LeaveRequest entity)
        {
            await _context.LeaveRequests.AddAsync(entity);
        }

        public async Task UpdateAsync(LeaveRequest entity)
        {
            _context.LeaveRequests.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}