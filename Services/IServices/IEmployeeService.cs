using SmartHR_Payroll.Models;
using SmartHR_Payroll.ViewModels.Profile;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IEmployeeService
    {
        Task<ProfileViewModel?> GetProfileAsync(int employeeId);
        Task<EditProfileViewModel?> GetEditProfileAsync(int employeeId);
        Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileViewModel model);
        Task<Employee?> GetByIdAsync(int id);
    }
}