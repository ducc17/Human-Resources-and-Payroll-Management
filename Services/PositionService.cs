using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Position;

namespace SmartHR_Payroll.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;

        public PositionService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        // ==========================================
        // LOGIC NGHIỆP VỤ: TỰ ĐỘNG SINH MÃ VỊ TRÍ
        // ==========================================
        private string GeneratePositionCode(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "POS-" + new Random().Next(100, 999);

            string[] vnSigns = { "aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ", "ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ" };
            for (int i = 1; i < vnSigns.Length; i++)
            {
                for (int j = 0; j < vnSigns[i].Length; j++)
                    name = name.Replace(vnSigns[i][j].ToString(), vnSigns[0][i - 1].ToString());
            }

            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string initials = "POS-";
            foreach (var w in words) initials += w[0];

            return initials.ToUpper() + new Random().Next(10, 99).ToString();
        }

        // ==========================================
        // CÁC HÀM XỬ LÝ
        // ==========================================
        public async Task<List<PositionListViewModel>> GetAllWithDetailsAsync(int? departmentId = null, string? sortBy = null)
            => await _positionRepository.GetAllWithDetailsAsync(departmentId, sortBy); public async Task<Position?> GetByIdAsync(int id) => await _positionRepository.GetByIdAsync(id);
        public async Task<List<Department>> GetDepartmentsForDropdownAsync() => await _positionRepository.GetDepartmentsForDropdownAsync();

        // LOGIC TẠO MỚI (Đã gom hết logic vào đây)
        public async Task CreateAsync(Position position, string currentUserName)
        {
            position.Code = GeneratePositionCode(position.Name);
            position.IsDeleted = false;

            position.CreatedBy = currentUserName;
            position.CreatedAt = DateTime.UtcNow;
            position.UpdatedBy = position.CreatedBy;
            position.UpdatedAt = position.CreatedAt;

            await _positionRepository.CreateAsync(position);
        }

        // LOGIC CẬP NHẬT
        public async Task UpdateAsync(Position position, string currentUserName)
        {
            position.UpdatedBy = currentUserName;
            position.UpdatedAt = DateTime.UtcNow;

            await _positionRepository.UpdateAsync(position);
        }

        public async Task DeactivateAsync(int id)
        {
            // Kiểm tra trước khi khóa
            bool isOccupied = await _positionRepository.HasEmployeesAsync(id);
            if (isOccupied)
            {
                // Quăng lỗi lên cho Controller chụp lại
                throw new InvalidOperationException("Không thể khóa! Đang có nhân viên đảm nhiệm vị trí này. Vui lòng chuyển công tác nhân viên trước.");
            }

            // Nếu không có ai thì khóa bình thường
            await _positionRepository.DeactivateAsync(id);
        }
    }
}