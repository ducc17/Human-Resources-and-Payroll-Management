using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Insurance;

namespace SmartHR_Payroll.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository _insuranceRepository;

        public InsuranceService(IInsuranceRepository insuranceRepository)
        {
            _insuranceRepository = insuranceRepository;
        }

        public async Task<List<InsuranceViewModel>> GetAllInsurancesAsync()
        {
            var list = await _insuranceRepository.GetAllAsync();
            return list.Select(i => new InsuranceViewModel
            {
                InsuranceId = i.InsuranceId,
                Code = i.Code,
                Name = i.Name,
                EmployeeRate = i.EmployeeRate,
                CompanyRate = i.CompanyRate,
                MaxSalaryLimit = i.MaxSalaryLimit
            }).ToList();
        }

        public async Task<InsuranceViewModel?> GetInsuranceByIdAsync(int id)
        {
            var i = await _insuranceRepository.GetByIdAsync(id);
            if (i == null) return null;
            return new InsuranceViewModel { InsuranceId = i.InsuranceId, Code = i.Code, Name = i.Name, EmployeeRate = i.EmployeeRate, CompanyRate = i.CompanyRate, MaxSalaryLimit = i.MaxSalaryLimit };
        }

        public async Task<bool> CreateInsuranceAsync(InsuranceViewModel model, string createdBy)
        {
            if (await _insuranceRepository.CheckCodeExistsAsync(model.Code)) return false; 

            var entity = new Insurance
            {
                Code = model.Code,
                Name = model.Name,
                EmployeeRate = model.EmployeeRate,
                CompanyRate = model.CompanyRate,
                MaxSalaryLimit = model.MaxSalaryLimit,
                CreatedBy = createdBy
            };
            await _insuranceRepository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateInsuranceAsync(InsuranceViewModel model, string updatedBy)
        {
            if (await _insuranceRepository.CheckCodeExistsAsync(model.Code, model.InsuranceId)) return false;

            var entity = await _insuranceRepository.GetByIdAsync(model.InsuranceId);
            if (entity == null) return false;

            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.EmployeeRate = model.EmployeeRate;
            entity.CompanyRate = model.CompanyRate;
            entity.MaxSalaryLimit = model.MaxSalaryLimit;
            entity.UpdatedBy = updatedBy;

            await _insuranceRepository.UpdateAsync(entity);
            return true;
        }

        public async Task DeleteInsuranceAsync(int id, string deletedBy) => await _insuranceRepository.DeleteAsync(id, deletedBy);

    }
}
