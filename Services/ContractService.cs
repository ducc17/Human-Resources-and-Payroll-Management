using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Contract;

namespace SmartHR_Payroll.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;

        public ContractService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<List<Employee>> GetEmployeesForCreateAsync()
        {
            return await _contractRepository.GetEmployeesForContractAsync();
        }

        public async Task<(bool Success, string Message)> CreateContractAsync(CreateContractViewModel model, string actor)
        {
            if (model == null)
            {
                return (false, "Dữ liệu hợp đồng không hợp lệ.");
            }

            if (!await _contractRepository.EmployeeExistsAsync(model.EmployeeId))
            {
                return (false, "Nhân viên không tồn tại.");
            }

            // Check if employee already has an active contract
            if (await _contractRepository.HasActiveContractAsync(model.EmployeeId))
            {
                return (false, "Hợp đồng với nhân viên này vẫn còn hạn. Vui lòng hủy hợp đồng hiện tại trước khi tạo hợp đồng mới.");
            }

            var contractNumber = await GenerateContractNumberAsync();

            if (model.EndDate.HasValue && model.EndDate.Value < model.StartDate)
            {
                return (false, "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            if (model.BaseSalary <= 0)
            {
                return (false, "Lương cơ bản phải lớn hơn 0.");
            }

            var contract = new Models.Contract
            {
                EmployeeId = model.EmployeeId,
                ContractNumber = contractNumber,
                Type = model.Type,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                BaseSalary = model.BaseSalary,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = actor,
                UpdatedAt = null,
                UpdatedBy = actor,
                IsDeleted = false
            };

            try
            {
                await _contractRepository.AddContractAsync(contract);
                return (true, "Tạo hợp đồng thành công.");
            }
            catch (Exception)
            {
                return (false, "Có lỗi xảy ra khi lưu hợp đồng. Vui lòng thử lại.");
            }
        }

        public async Task<(bool Success, string Message)> CancelContractAsync(int contractId, string actor)
        {
            var contract = await _contractRepository.GetContractByIdAsync(contractId);
            if (contract == null)
            {
                return (false, "Không tìm thấy hợp đồng.");
            }

            if (!contract.IsActive)
            {
                return (false, "Hợp đồng này đã được hủy trước đó.");
            }

            contract.IsActive = false;
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (!contract.EndDate.HasValue || contract.EndDate.Value > today)
            {
                contract.IsDeleted = true;
            }

            contract.UpdatedAt = DateTime.UtcNow;
            contract.UpdatedBy = actor;

            try
            {
                await _contractRepository.UpdateContractAsync(contract);
                return (true, "Hủy hợp đồng thành công.");
            }
            catch
            {
                return (false, "Không thể hủy hợp đồng. Vui lòng thử lại.");
            }
        }

        public async Task<(bool Success, string Message)> ConfirmContractByEmployeeAsync(int contractId, int employeeId, string actor)
        {
            var contract = await _contractRepository.GetContractByIdAsync(contractId);
            if (contract == null)
            {
                return (false, "Không tìm thấy hợp đồng.");
            }

            if (contract.EmployeeId != employeeId)
            {
                return (false, "Bạn không có quyền xác nhận hợp đồng này.");
            }

            if (contract.IsActive)
            {
                return (false, "Hợp đồng này đã được xác nhận trước đó.");
            }

            contract.IsActive = true;
            contract.IsDeleted = false;
            contract.UpdatedAt = DateTime.UtcNow;
            contract.UpdatedBy = actor;

            try
            {
                await _contractRepository.UpdateContractAsync(contract);
                return (true, "Xác nhận hợp đồng thành công.");
            }
            catch
            {
                return (false, "Không thể xác nhận hợp đồng. Vui lòng thử lại.");
            }
        }

        private async Task<string> GenerateContractNumberAsync()
        {
            while (true)
            {
                var candidate = $"HD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
                if (!await _contractRepository.ContractNumberExistsAsync(candidate))
                {
                    return candidate;
                }

                await Task.Delay(1);
            }
        }
    }
}
