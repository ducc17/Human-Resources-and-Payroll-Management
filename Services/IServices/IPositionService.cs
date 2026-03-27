using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Position;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IPositionService
    {
        Task<List<PositionListViewModel>> GetAllWithDetailsAsync(int? departmentId = null, string? sortBy = null); Task<Position?> GetByIdAsync(int id);
        Task<List<Department>> GetDepartmentsForDropdownAsync();
        Task CreateAsync(Position position, string currentUserName);
        Task UpdateAsync(Position position, string currentUserName);
        Task DeactivateAsync(int id);
    }
}