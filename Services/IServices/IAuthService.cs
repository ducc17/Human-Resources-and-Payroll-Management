using System.Security.Claims;

namespace SmartHR_Payroll.Services.IServices
{
    public interface IAuthService
    {
        Task<ClaimsPrincipal> ValidateGoogleUserAsync(string email, string avatarUrl);
    }
}
