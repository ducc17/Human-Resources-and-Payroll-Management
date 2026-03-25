using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;

namespace SmartHR_Payroll.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DBCodeFirstContext _context;

        public EmployeeRepository(DBCodeFirstContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeProfileAsync(int employeeId)
        {
            // Include để lấy luôn thông tin tên Phòng ban và Vị trí thay vì chỉ lấy ID
            return await _context.Employees
                // .Include(e => e.Department) // Bỏ comment 2 dòng này nếu bạn đã setup Navigation Property trong Model
                // .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
    }
}