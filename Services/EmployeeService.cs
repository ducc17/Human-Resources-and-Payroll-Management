using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Profile;
using SmartHR_Payroll.ViewModels.Employee;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

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
                Gender = emp.Gender == Status.Gender.Male ? "Nam" : (emp.Gender == Status.Gender.Female ? "Nữ" : "Khác"),
                Email = emp.Email,
                PhoneNumber = emp.PhoneNumber,
                Address = emp.Address,
                BankName = emp.Bank?.BankName ?? string.Empty,
                BankAccountNumber = emp.BankAccountNumber,
                HireDate = emp.HireDate,
                DepartmentName = emp.Job?.Department?.Name ?? "Chưa xếp phòng",
                PositionName = emp.Job?.Position?.Name ?? "Chưa có chức vụ"
            };
        }

        public async Task<List<Bank>> GetAllBanksAsync()
        {
            // Lấy danh sách ngân hàng chưa bị xóa, sắp xếp theo tên viết tắt (VCB, BIDV...)
            return await _employeeRepository.GetAllBanksAsync();
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
                BankId = emp.BankId,
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
            emp.BankAccountNumber = model.BankAccountNumber;

            // ĐÃ FIX: Gán thẳng BankId từ Form (Dropdown) gửi lên. 
            // Nếu người dùng chọn "-- Chọn ngân hàng --" thì model.BankId sẽ là null, 
            // hệ thống cũng tự hiểu và lưu null xuống DB.
            emp.BankId = model.BankId;

            await _employeeRepository.UpdateEmployeeAsync(emp);
            return (true, "Cập nhật hồ sơ thành công!");
        }

        public async Task<(List<Job> Jobs, List<Bank> Banks, List<Role> Roles)> GetCreateEmployeeLookupsAsync()
        {
            var jobs = await _employeeRepository.GetJobsAsync();
            var banks = await _employeeRepository.GetBanksAsync();
            var roles = await _employeeRepository.GetRolesAsync();

            return (jobs, banks, roles);
        }

        public async Task<(bool Success, string Message)> CreateEmployeeAsync(Employee employee, string actor)
        {
            if (employee == null)
                return (false, "Dữ liệu nhân viên không hợp lệ.");

            if (new[] { employee.FirstName, employee.LastName, employee.Email, employee.PhoneNumber,
                employee.Address, employee.BankAccountNumber }
                .Any(string.IsNullOrWhiteSpace))
                return (false, "Vui lòng điền đầy đủ các thông tin bắt buộc.");

            if (employee.JobId <= 0 || employee.RoleId <= 0)
                return (false, "Vị trí công việc và vai trò không được để trống.");

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (employee.DateOfBirth >= today)
                return (false, "Ngày sinh phải trước ngày hôm nay.");

            if (employee.HireDate > today)
                return (false, "Ngày vào làm không được là ngày tương lai.");

            if (employee.DateOfBirth.Year < 1950)
                return (false, "Ngày sinh không hợp lệ (năm sinh phải từ 1950).");

            employee.FirstName = employee.FirstName.Trim();
            employee.LastName = employee.LastName.Trim();
            employee.Email = employee.Email.Trim();
            employee.PhoneNumber = employee.PhoneNumber.Trim();
            employee.Address = employee.Address.Trim();
            employee.BankAccountNumber = employee.BankAccountNumber.Trim();

            if (!IsLettersOnly(employee.FirstName))
            {
                return (false, "First name chỉ được chứa chữ cái.");
            }

            if (!IsLettersOnly(employee.LastName))
            {
                return (false, "Last name chỉ được chứa chữ cái.");
            }

            if (!IsValidEmail(employee.Email))
            {
                return (false, "Email không đúng định dạng.");
            }

            if (!IsValidPhoneNumber(employee.PhoneNumber))
            {
                return (false, "Số điện thoại không đúng định dạng.");
            }

            if (!IsValidAddress(employee.Address))
            {
                return (false, "Địa chỉ chỉ được chứa chữ, số và dấu phẩy.");
            }

            if (await _employeeRepository.EmailExistsAsync(employee.Email))
            {
                return (false, "Email đã tồn tại trong hệ thống.");
            }

            if (await _employeeRepository.PhoneNumberExistsAsync(employee.PhoneNumber))
            {
                return (false, "Số điện thoại đã tồn tại trong hệ thống.");
            }

            if (await _employeeRepository.BankAccountNumberExistsAsync(employee.BankAccountNumber))
            {
                return (false, "Số tài khoản ngân hàng đã tồn tại trong hệ thống.");
            }

            var jobs = await _employeeRepository.GetJobsAsync();
            if (!jobs.Any(j => j.JobId == employee.JobId))
            {
                return (false, "Công việc không hợp lệ.");
            }

            var roles = await _employeeRepository.GetRolesAsync();
            if (!roles.Any(r => r.RoleId == employee.RoleId))
            {
                return (false, "Vai trò không hợp lệ.");
            }

            employee.Gender = employee.Gender;
            employee.Status = Status.EmployeeStatus.Active;
            employee.RoleId = employee.RoleId;
            employee.CreatedAt = DateTime.UtcNow;
            employee.CreatedBy = actor;
            employee.UpdatedAt = null;
            employee.UpdatedBy = "System";
            employee.IsDeleted = false;


            employee.EmployeeCode = await GenerateEmployeeCodeAsync();

            try
            {
                await _employeeRepository.AddEmployeeAsync(employee);
                return (true, "Thêm nhân viên thành công.");
            }
            catch (DbUpdateException)
            {
                return (false, "Lỗi hệ thống khi lưu dữ liệu. Vui lòng thử lại.");
            }
            catch (Exception)
            {
                return (false, "Lỗi hệ thống. Vui lòng thử lại.");
            }


        }

        public async Task<EmployeeListViewModel> GetEmployeesPagedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (employees, totalCount) = await _employeeRepository.GetEmployeesPagedAsync(page, pageSize);
            var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new EmployeeListViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Employees = employees.Select(e => new EmployeeListItemViewModel
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    FullName = e.FullName,
                    DateOfBirth = e.DateOfBirth,
                    HireDate = e.HireDate,
                    Gender = e.Gender,
                    Address = e.Address,
                    CreatedAt = e.CreatedAt,
                    CreatedBy = e.CreatedBy ?? string.Empty,
                    UpdatedAt = e.UpdatedAt,
                    UpdatedBy = e.UpdatedBy ?? string.Empty,
                    DepartmentName = e.Job?.Department?.Name ?? string.Empty,
                    PositionName = e.Job?.Position?.Name ?? string.Empty,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Status = e.Status
                }).ToList()
            };
        }

        public async Task<(bool Success, string Message)> BanEmployeeAsync(int employeeId, string actor)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return (false, "Không tìm thấy nhân viên.");
            }

            if (employee.Status == Status.EmployeeStatus.Terminated)
            {
                return (false, "Nhân viên này đã bị ban trước đó.");
            }

            employee.Status = Status.EmployeeStatus.Terminated;
            employee.IsDeleted = false;
            employee.UpdatedAt = DateTime.UtcNow;
            employee.UpdatedBy = actor;

            try
            {
                await _employeeRepository.UpdateEmployeeAsync(employee);
                return (true, "Ban account nhân viên thành công.");
            }
            catch
            {
                return (false, "Không thể ban account. Vui lòng thử lại.");
            }
        }

        public async Task<(bool Success, string Message)> UnbanEmployeeAsync(int employeeId, string actor)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return (false, "Không tìm thấy nhân viên.");
            }

            if (employee.Status != Status.EmployeeStatus.Terminated)
            {
                return (false, "Nhân viên này chưa bị ban.");
            }

            employee.Status = Status.EmployeeStatus.Active;
            employee.IsDeleted = false;
            employee.UpdatedAt = DateTime.UtcNow;
            employee.UpdatedBy = actor;

            try
            {
                await _employeeRepository.UpdateEmployeeAsync(employee);
                return (true, "Unban account nhân viên thành công.");
            }
            catch
            {
                return (false, "Không thể unban account. Vui lòng thử lại.");
            }
        }

        public async Task<EmployeeContractsViewModel?> GetEmployeeContractsAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return null;
            }

            var contracts = await _employeeRepository.GetContractsByEmployeeIdAsync(employeeId);
            return new EmployeeContractsViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Contracts = contracts.Select(c => new EmployeeContractItemViewModel
                {
                    ContractId = c.ContractId,
                    ContractNumber = c.ContractNumber,
                    Type = c.Type,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    BaseSalary = c.BaseSalary,
                    IsActive = c.IsActive
                }).ToList()
            };
        }

        private async Task<string> GenerateEmployeeCodeAsync()
        {
            var code = $"EMP{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            if (!await _employeeRepository.EmployeeCodeExistsAsync(code))
            {
                return code;
            }
            await Task.Delay(1);
            return $"EMP{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        private static bool IsLettersOnly(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && Regex.IsMatch(value, @"^[\p{L}]+(?:\s+[\p{L}]+)*$", RegexOptions.CultureInvariant);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var emailAttr = new EmailAddressAttribute();
            return emailAttr.IsValid(email);
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber)
                && Regex.IsMatch(phoneNumber, @"^(\+84|0)[0-9]{9}$", RegexOptions.CultureInvariant);
        }

        private static bool IsValidAddress(string address)
        {
            return !string.IsNullOrWhiteSpace(address)
                && Regex.IsMatch(address, @"^[\p{L}0-9,\s\-()./]+$", RegexOptions.CultureInvariant);
        }
        public async Task<Employee?> GetByIdAsync(int id)
        {
            // Giả định trong EmployeeRepository của bạn có hàm GetByIdAsync
            return await _employeeRepository.GetByIdAsync(id);
        }
    }
}