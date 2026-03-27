using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using System.Security.Claims;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _repo;

        public LeaveRequestService(ILeaveRequestRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LeaveRequest>> GetAllAsync(
            string? search,
            DateOnly? fromDate,
            DateOnly? toDate,
            string? status,
            ClaimsPrincipal user)
        {
            int? departmentId = null;
            int? employeeId = null;

            // Manager -> theo phòng ban
            if (user.IsInRole("Manager"))
            {
                var dept = user.FindFirst("DepartmentId")?.Value;
                if (int.TryParse(dept, out int d))
                    departmentId = d;
            }

            // Employee -> chỉ thấy của mình
            if (user.IsInRole("Employee"))
            {
                var emp = user.FindFirst("EmployeeId")?.Value;
                if (int.TryParse(emp, out int e))
                    employeeId = e;
            }

            return await _repo.GetAllAsync(
                search,
                fromDate,
                toDate,
                status,
                departmentId,
                employeeId
            );
        }

        public async Task CreateAsync(LeaveRequest model, ClaimsPrincipal user)
        {
            var empId = user.FindFirst("EmployeeId")?.Value;

            if (empId == null)
                throw new Exception("Không xác định được nhân viên");

            model.EmployeeId = int.Parse(empId);

            model.Status = LeaveStatus.Pending;
            model.CreatedAt = DateTime.Now;

            // validate ngày
            if (model.EndDate < model.StartDate)
                throw new Exception("Ngày kết thúc phải >= ngày bắt đầu");

            // tính số ngày
            model.TotalDays = (model.EndDate.DayNumber - model.StartDate.DayNumber) + 1;

            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();
        }

        public async Task ApproveAsync(int id, ClaimsPrincipal user)
        {
            if (!user.IsInRole("Manager") && !user.IsInRole("HR"))
                throw new Exception("Không có quyền");

            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Không tìm thấy đơn");

            entity.Status = LeaveStatus.Approved;

            var approver = user.FindFirst("EmployeeId")?.Value;
            if (approver != null)
                entity.ApprovedById = int.Parse(approver);

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task RejectAsync(int id, ClaimsPrincipal user)
        {
            if (!user.IsInRole("Manager") && !user.IsInRole("HR"))
                throw new Exception("Không có quyền");

            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Không tìm thấy đơn");

            entity.Status = LeaveStatus.Rejected;

            var approver = user.FindFirst("EmployeeId")?.Value;
            if (approver != null)
                entity.ApprovedById = int.Parse(approver);

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
        }
    }
}