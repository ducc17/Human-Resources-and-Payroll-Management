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
        private readonly IContractRepository _contractRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IContractRepository contractRepository)
        {
            _employeeRepository = employeeRepository;
            _contractRepository = contractRepository;
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

        public async Task<EditProfileViewModel?> GetEditProfileAsync(int employeeId)
        {
            var emp = await _employeeRepository.GetEmployeeProfileAsync(employeeId);
            if (emp == null) return null;

            return new EditProfileViewModel
            {
                EmployeeId = emp.EmployeeId,
                PhoneNumber = emp.PhoneNumber,
                Address = emp.Address,
                BankName = emp.Bank?.BankName ?? string.Empty,
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

            if (!string.IsNullOrWhiteSpace(model.BankName))
            {
                var banks = await _employeeRepository.GetBanksAsync();
                var selectedBank = banks.FirstOrDefault(b =>
                    string.Equals(b.BankName, model.BankName.Trim(), StringComparison.OrdinalIgnoreCase)
                    || string.Equals(b.ShortName, model.BankName.Trim(), StringComparison.OrdinalIgnoreCase)
                    || string.Equals(b.BankCode, model.BankName.Trim(), StringComparison.OrdinalIgnoreCase));

                if (selectedBank == null)
                {
                    return (false, "Ngân hàng không hợp lệ.");
                }

                emp.BankId = selectedBank.BankId;
            }

            await _employeeRepository.UpdateEmployeeAsync(emp);
            return (true, "Cập nhật hồ sơ thành công!");
        }

        public async Task<(List<Job> Jobs, List<Bank> Banks, List<Role> Roles)> GetCreateEmployeeLookupsAsync()
        {
            var jobs = await _employeeRepository.GetJobsAsync();
            var banks = await _employeeRepository.GetBanksAsync();
            var roles = (await _employeeRepository.GetRolesAsync())
                .Where(r => !string.Equals(r.Name, "Admin", StringComparison.OrdinalIgnoreCase))
                .ToList();

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

            if (roles.Any(r => r.RoleId == employee.RoleId
                && string.Equals(r.Name, "Admin", StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Không thể tạo nhân viên với vai trò Admin.");
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

        public async Task<EmployeeListViewModel> GetEmployeesPagedAsync(
            int page,
            int pageSize,
            int? departmentId,
            string keyword,
            string status)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (employees, totalCount) = await _employeeRepository
                .GetEmployeesPagedAsync(page, pageSize, departmentId, keyword, status);

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
                    RoleName = e.Role?.Name ?? string.Empty,
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
                return (false, "Nhân viên này đã bị khóa trước đó.");
            }

            // Check if employee has an active contract
            if (await _contractRepository.HasActiveContractAsync(employeeId))
            {
                return (false, "Không thể khóa tài khoản nhân viên này vì họ có hợp đồng còn hạn. Vui lòng hủy hợp đồng trước.");
            }

            employee.Status = Status.EmployeeStatus.Terminated;
            employee.IsDeleted = false;
            employee.UpdatedAt = DateTime.UtcNow;
            employee.UpdatedBy = actor;

            try
            {
                await _employeeRepository.UpdateEmployeeAsync(employee);
                return (true, "Khóa tài khoản nhân viên thành công.");
            }
            catch
            {
                return (false, "Không thể khóa tài khoản nhân viên. Vui lòng thử lại.");
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
                return (false, "Nhân viên này chưa bị khóa.");
            }

            employee.Status = Status.EmployeeStatus.Active;
            employee.IsDeleted = false;
            employee.UpdatedAt = DateTime.UtcNow;
            employee.UpdatedBy = actor;

            try
            {
                await _employeeRepository.UpdateEmployeeAsync(employee);
                return (true, "Mở khóa tài khoản nhân viên thành công.");
            }
            catch
            {
                return (false, "Không thể mở khóa tài khoản nhân viên. Vui lòng thử lại.");
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
                    IsActive = c.IsActive,
                    IsDeleted = c.IsDeleted
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

        public async Task<EmployeeAllowanceListViewModel?> GetEmployeeAllowancesAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return null;
            }

            var items = await _employeeRepository.GetEmployeeAllowancesAsync(employeeId);

            return new EmployeeAllowanceListViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Items = items.Select(ea => new EmployeeAllowanceItemViewModel
                {
                    AllowanceName = ea.Allowance?.Name ?? string.Empty,
                    Amount = ea.Amount,
                    EffectiveDate = ea.EffectiveDate
                }).ToList()
            };
        }

        public async Task<List<Allowance>> GetActiveAllowancesAsync()
        {
            return await _employeeRepository.GetActiveAllowancesAsync();
        }

        public async Task<(bool Success, string Message)> AddEmployeeAllowanceAsync(int employeeId, int allowanceId, decimal amount, DateOnly effectiveDate, string actor)
        {
            System.Diagnostics.Debug.WriteLine($"AddEmployeeAllowanceAsync called: EmployeeId={employeeId}, AllowanceId={allowanceId}, Amount={amount}, EffectiveDate={effectiveDate}");

            if (amount <= 0)
            {
                return (false, "Số tiền phụ cấp phải lớn hơn 0.");
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return (false, "Không tìm thấy nhân viên.");
            }

            var allowances = await _employeeRepository.GetActiveAllowancesAsync();
            var allowance = allowances.FirstOrDefault(a => a.AllowanceId == allowanceId);
            if (allowance == null)
            {
                return (false, "Khoản phụ cấp không hợp lệ.");
            }

            var existing = await _employeeRepository.GetEmployeeAllowanceAsync(employeeId, allowanceId, effectiveDate);
            if (existing == null)
            {
                System.Diagnostics.Debug.WriteLine("Creating new EmployeeAllowance...");
                existing = new EmployeeAllowance
                {
                    EmployeeId = employeeId,
                    AllowanceId = allowanceId,
                    EffectiveDate = effectiveDate,
                    Amount = amount
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Updating existing EmployeeAllowance (Id={existing.Id})...");
                existing.Amount = amount;
            }

            try
            {
                await _employeeRepository.SaveEmployeeAllowanceAsync(existing);
                System.Diagnostics.Debug.WriteLine("Successfully saved EmployeeAllowance");
                return (true, $"Đã lưu phụ cấp {allowance.Name} cho {employee.FullName} ({amount:N0}).");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in SaveEmployeeAllowanceAsync: {ex.Message}");
                return (false, $"Lỗi lưu dữ liệu: {ex.Message}");
            }
        }

        public async Task<EmployeeDeductionListViewModel?> GetEmployeeDeductionsAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return null;
            }

            var items = await _employeeRepository.GetEmployeeDeductionsAsync(employeeId);

            return new EmployeeDeductionListViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Items = items.Select(ed => new EmployeeDeductionItemViewModel
                {
                    DeductionName = ed.Deduction?.Name ?? string.Empty,
                    Amount = ed.Amount,
                    EffectiveDate = ed.EffectiveDate
                }).ToList()
            };
        }

        public async Task<List<Deduction>> GetActiveDeductionsAsync()
        {
            return await _employeeRepository.GetActiveDeductionsAsync();
        }

        public async Task<(bool Success, string Message)> AddEmployeeDeductionAsync(int employeeId, int deductionId, decimal amount, DateOnly effectiveDate, string actor)
        {
            if (amount <= 0)
            {
                return (false, "Số tiền khoản trừ phải lớn hơn 0.");
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return (false, "Không tìm thấy nhân viên.");
            }

            var deductions = await _employeeRepository.GetActiveDeductionsAsync();
            var deduction = deductions.FirstOrDefault(d => d.DeductionId == deductionId);
            if (deduction == null)
            {
                return (false, "Khoản trừ không hợp lệ.");
            }

            var existing = await _employeeRepository.GetEmployeeDeductionAsync(employeeId, deductionId, effectiveDate);
            if (existing == null)
            {
                existing = new EmployeeDeduction
                {
                    EmployeeId = employeeId,
                    DeductionId = deductionId,
                    EffectiveDate = effectiveDate,
                    Amount = amount
                };
            }
            else
            {
                existing.Amount = amount;
            }

            try
            {
                await _employeeRepository.SaveEmployeeDeductionAsync(existing);
                return (true, $"Đã lưu khoản trừ {deduction.Name} cho {employee.FullName} ({amount:N0}).");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi lưu dữ liệu: {ex.Message}");
            }
        }
    }
}