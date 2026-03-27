using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Position;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IPositionRepository
    {
        Task<List<PositionListViewModel>> GetAllWithDetailsAsync(int? departmentId = null, string? sortBy = null);
        Task<Position?> GetByIdAsync(int id);
        Task<List<Department>> GetDepartmentsForDropdownAsync(); // Lấy phòng ban cho Select
        Task CreateAsync(Position position);
        Task UpdateAsync(Position position);
        Task DeactivateAsync(int id);
        Task<bool> HasEmployeesAsync(int positionId);
    }
}