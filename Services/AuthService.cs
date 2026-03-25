using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Services.IServices;
using System.Security.Claims;
using static SmartHR_Payroll.Models.Status;

namespace SmartHR_Payroll.Services
{
    public class AuthService : IAuthService
    {
        private readonly DBCodeFirstContext _context;

        public AuthService(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<ClaimsPrincipal> ValidateGoogleUserAsync(string email, string avatarUrl)
        {
            var employee = await _context.Employees
                .Include(e => e.Role)      
                .Include(e => e.Position)  
                .Include(e => e.Department) 
                .FirstOrDefaultAsync(e => e.Email == email);

            if (employee == null || employee.Status != EmployeeStatus.Active)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
                new Claim(ClaimTypes.Name, employee.FullName),
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role.Name ?? "Employee"),
                new Claim("DepartmentCode", employee.Department.Code),
                new Claim("PositionName", employee.Position.Name),
                new Claim("Avatar", avatarUrl ?? "")
            };

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");

            return new ClaimsPrincipal(identity);   
        }
    }
}
