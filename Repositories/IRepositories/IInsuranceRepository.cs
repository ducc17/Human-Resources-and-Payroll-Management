using SmartHR_Payroll.Models;

namespace SmartHR_Payroll.Repositories.IRepositories
{
    public interface IInsuranceRepository
    {
        Task<List<Insurance>> GetAllAsync();
        Task<Insurance?> GetByIdAsync(int id);
        Task<bool> CheckCodeExistsAsync(string code, int excludeId = 0);
        Task AddAsync(Insurance insurance);
        Task UpdateAsync(Insurance insurance);
        Task DeleteAsync(int id, string deletedBy);
    }
}
