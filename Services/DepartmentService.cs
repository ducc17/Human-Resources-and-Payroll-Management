using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Department;

namespace SmartHR_Payroll.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public async Task<DepartmentDetailViewModel?> GetDepartmentDetailAsync(int id)
        {
            return await _departmentRepository.GetDepartmentDetailAsync(id);
        }

        // ==========================================
        // CHUYỂN LOGIC SINH MÃ TỪ CONTROLLER SANG ĐÂY
        // ==========================================
        private string GenerateDepartmentCode(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "DEP" + new Random().Next(100, 999);

            string[] vnSigns = { "aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ", "ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ" };
            for (int i = 1; i < vnSigns.Length; i++)
            {
                for (int j = 0; j < vnSigns[i].Length; j++)
                    name = name.Replace(vnSigns[i][j].ToString(), vnSigns[0][i - 1].ToString());
            }

            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string initials = "";
            foreach (var w in words) initials += w[0];

            return initials.ToUpper() + new Random().Next(100, 999).ToString();
        }

        public async Task<List<DepartmentListViewModel>> GetAllWithDetailsAsync(string? sortBy = null)
            => await _departmentRepository.GetAllWithDetailsAsync(sortBy); public async Task<Department?> GetByIdAsync(int id) => await _departmentRepository.GetByIdAsync(id);
        public async Task<List<Employee>> GetEmployeesForDropdownAsync(int? currentManagerId = null) => await _departmentRepository.GetEmployeesForDropdownAsync(currentManagerId);

        // LOGIC TẠO MỚI (Tự sinh mã và gán lịch sử)
        public async Task CreateAsync(Department department, string currentUserName)
        {
            department.Code = GenerateDepartmentCode(department.Name);
            department.IsDeleted = false;

            department.CreatedBy = currentUserName;
            department.CreatedAt = DateTime.UtcNow;
            department.UpdatedBy = department.CreatedBy;
            department.UpdatedAt = department.CreatedAt;

            await _departmentRepository.CreateAsync(department);
        }

        // LOGIC CẬP NHẬT
        public async Task UpdateAsync(Department department, string currentUserName)
        {
            department.UpdatedBy = currentUserName;
            department.UpdatedAt = DateTime.UtcNow;

            await _departmentRepository.UpdateAsync(department);
        }
        public async Task<bool> CheckManagerConflictAsync(int managerId, int currentDepartmentId)
        {
            return await _departmentRepository.CheckManagerConflictAsync(managerId, currentDepartmentId);
        }

        public async Task DeactivateAsync(int id) => await _departmentRepository.DeactivateAsync(id);
    }
}