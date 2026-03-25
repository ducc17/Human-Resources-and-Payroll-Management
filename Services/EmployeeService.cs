using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Profile;

namespace SmartHR_Payroll.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(int employeeId)
        {
            var emp = await _employeeRepository.GetEmployeeProfileAsync(employeeId);
            if (emp == null) return null;

            return new ProfileViewModel
            {
                EmployeeId = emp.EmployeeId,
                EmployeeCode = emp.EmployeeCode,
                FullName = $"{emp.FirstName} {emp.LastName}",
                DateOfBirth = emp.DateOfBirth,
                // Trong hàm GetProfileAsync
                Gender = emp.Gender == Status.Gender.Male ? "Nam" :(emp.Gender == Status.Gender.Female ? "Nữ" : "Khác"),
                Email = emp.Email,
                PhoneNumber = emp.PhoneNumber,
                Address = emp.Address,
                BankName = emp.BankName,
                BankAccountNumber = emp.BankAccountNumber,
                HireDate = emp.HireDate,
                // DepartmentName = emp.Department?.name ?? "Chưa xếp phòng", // Bỏ comment nếu có Navigation Property
                // PositionName = emp.Position?.name ?? "Chưa có chức vụ"
                DepartmentName = "Phòng IT", // Dữ liệu giả định tạm thời
                PositionName = "Developer"   // Dữ liệu giả định tạm thời
            };
        }

        public async Task<EditProfileViewModel?> GetEditProfileAsync(int employeeId)
        {
            var emp = await _employeeRepository.GetEmployeeProfileAsync(employeeId);
            if (emp == null) return null;

            return new EditProfileViewModel
            {
                EmployeeId = emp.EmployeeId,
                PhoneNumber = emp.PhoneNumber,
                Address = emp.Address,
                BankName = emp.BankName,
                BankAccountNumber = emp.BankAccountNumber
            };
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(EditProfileViewModel model)
        {
            var emp = await _employeeRepository.GetEmployeeProfileAsync(model.EmployeeId);
            if (emp == null) return (false, "Không tìm thấy thông tin nhân viên.");

            // CHỈ cập nhật những trường cho phép (Bảo mật)
            emp.PhoneNumber = model.PhoneNumber;
            emp.Address = model.Address;
            emp.BankName = model.BankName;
            emp.BankAccountNumber = model.BankAccountNumber;

            await _employeeRepository.UpdateEmployeeAsync(emp);
            return (true, "Cập nhật hồ sơ thành công!");
        }
    }
}