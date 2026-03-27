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

            if (string.IsNullOrWhiteSpace(model.ContractNumber))
            {
                return (false, "Mã hợp đồng không được để trống.");
            }

            var contractNumber = model.ContractNumber.Trim();
            if (await _contractRepository.ContractNumberExistsAsync(contractNumber))
            {
                return (false, "Mã hợp đồng đã tồn tại.");
            }

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
                IsActive = model.IsActive,
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
    }
}
