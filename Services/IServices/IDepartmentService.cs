using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Department; // Bắt buộc phải có để dùng ViewModel

namespace SmartHR_Payroll.Services.IServices
{
    public interface IDepartmentService
    {
        // Lấy danh sách phòng ban kèm số lượng NV và thông tin Manager
        Task<List<DepartmentListViewModel>> GetAllWithDetailsAsync(string? sortBy = null);
        // Lấy thông tin 1 phòng ban theo ID (Dùng cho hàm Edit)
        Task<DepartmentDetailViewModel?> GetDepartmentDetailAsync(int id);
        Task<Department?> GetByIdAsync(int id);

        // Lấy danh sách nhân viên đang hoạt động để nhét vào Dropdown chọn Manager
        Task<List<Employee>> GetEmployeesForDropdownAsync(int? currentManagerId = null);

        Task CreateAsync(Department department, string currentUserName);
        Task UpdateAsync(Department department, string currentUserName);

        Task<bool> CheckManagerConflictAsync(int managerId, int currentDepartmentId);
        Task DeactivateAsync(int id);
        Task AssignManagerAsync(int departmentId, int employeeId, string currentUserName);
    }
}