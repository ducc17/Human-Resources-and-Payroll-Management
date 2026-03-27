using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using SmartHR_Payroll.Models;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services.IServices;
using SmartHR_Payroll.ViewModels.Attendance;
using System.ComponentModel;

namespace SmartHR_Payroll.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;

            ExcelPackage.License.SetNonCommercialPersonal("SmartHR Project");

        }

        public async Task<List<Attendance>> GetMyAttendanceHistoryAsync(int employeeId, DateOnly? fromDate, DateOnly? toDate, string? status)
        {
            return await _attendanceRepository.GetMyAttendanceHistoryAsync(employeeId, fromDate, toDate, status);
        }

        public async Task<(int SuccessCount, int ErrorCount, List<string> ErrorMessages)> ImportExcelAsync(IFormFile file)
        {
            int successCount = 0;
            int errorCount = 0;
            var errorMessages = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    // Lấy Sheet đầu tiên
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null) return (0, 0, new List<string> { "File Excel không có dữ liệu." });

                    int rowCount = worksheet.Dimension.Rows;

                    // Duyệt từ dòng 2 (bỏ qua tiêu đề ở dòng 1)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            string empCode = worksheet.Cells[row, 1].Text?.Trim();
                            string dateStr = worksheet.Cells[row, 2].Text?.Trim();
                            string checkInStr = worksheet.Cells[row, 3].Text?.Trim();
                            string checkOutStr = worksheet.Cells[row, 4].Text?.Trim();
                            string excelNote = worksheet.Cells[row, 5].Text?.Trim();
                            if (string.IsNullOrEmpty(empCode)) continue;

                            var employee = await _attendanceRepository.GetEmployeeByCodeAsync(empCode);
                            if (employee == null)
                            {
                                errorCount++;
                                errorMessages.Add($"Dòng {row}: Không tìm thấy mã NV '{empCode}'.");
                                continue;
                            }

                            // Khai báo các định dạng ngày phổ biến ở Việt Nam (có dùng dấu / hoặc dấu -)
                            string[] dateFormats = { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };

                            // Sử dụng TryParseExact để ép C# đọc theo đúng định dạng trên
                            if (!DateOnly.TryParseExact(dateStr, dateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateOnly date))
                            {
                                // Bổ sung thêm trường hợp: Nếu Excel lưu ngày dưới dạng số Serial (vd: 45374)
                                if (double.TryParse(dateStr, out double serialDate))
                                {
                                    date = DateOnly.FromDateTime(DateTime.FromOADate(serialDate));
                                }
                                else
                                {
                                    errorCount++;
                                    errorMessages.Add($"Dòng {row}: Định dạng ngày không hợp lệ '{dateStr}'. Vui lòng định dạng cột Ngày thành dd/MM/yyyy.");
                                    continue;
                                }
                            }

                            TimeSpan? checkInTime = string.IsNullOrEmpty(checkInStr) ? null : TimeSpan.Parse(checkInStr);
                            TimeSpan? checkOutTime = string.IsNullOrEmpty(checkOutStr) ? null : TimeSpan.Parse(checkOutStr);

                            decimal totalHours = 0;
                            if (checkInTime.HasValue && checkOutTime.HasValue)
                            {
                                totalHours = (decimal)(checkOutTime.Value - checkInTime.Value).TotalHours;
                                totalHours = Math.Round(totalHours, 2);
                            }

                            bool isLate = checkInTime.HasValue && checkInTime.Value > new TimeSpan(8, 30, 0);
                            string finalNote = !string.IsNullOrEmpty(excelNote)
                                        ? excelNote
                                        : "Import từ file Excel IoT";
                            // Khởi tạo đối tượng bằng các thuộc tính snake_case khớp với DB
                            var attendanceRecord = new Attendance
                            {
                                EmployeeId = employee.EmployeeId,
                                Date = date,
                                CheckInTime = checkInTime,
                                CheckOutTime = checkOutTime,
                                TotalHours = totalHours,
                                IsLate = isLate,
                                Note = finalNote
                            };

                            await _attendanceRepository.AddMultipleAttendanceAsync(attendanceRecord);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            errorMessages.Add($"Dòng {row}: Lỗi xử lý dữ liệu ({ex.Message}).");
                        }
                    }
                }
            }

            return (successCount, errorCount, errorMessages);
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _attendanceRepository.GetAllDepartmentsAsync();
        }

        public async Task<List<DailyAttendanceViewModel>> GetAllAttendancesAsync(string? search, DateOnly? fromDate, DateOnly? toDate, string? status, int? departmentId)
        {
            return await _attendanceRepository.GetAllAttendancesAsync(search, fromDate, toDate, status, departmentId);
        }

        public async Task<(List<DailyAttendanceViewModel> Items, int TotalPages)> GetAllAttendancesAsync(
            string? search,
            DateOnly? fromDate,
            DateOnly? toDate,
            string? status,
            int? departmentId,
            int page = 1,
            int pageSize = 10)
        {
            // 1. Lấy toàn bộ dữ liệu đã lọc và gom nhóm từ Repository
            var allData = await _attendanceRepository.GetAllAttendancesAsync(search, fromDate, toDate, status, departmentId);

            // 2. LOGIC PHÂN TRANG (Đã được chuyển xuống đây)
            int totalRecords = allData.Count;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Cắt lấy đúng số lượng dòng của trang hiện tại
            var pagedData = allData.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // 3. Trả về cả Data và Tổng số trang
            return (pagedData, totalPages);
        }
        public async Task<Dictionary<int, DailyAttendanceViewModel>> GetMyAttendanceCalendarAsync(int employeeId, int month, int year)
        {
            // 1. Tính toán ngày bắt đầu và kết thúc của tháng
            var startDate = new DateOnly(year, month, 1);
            var endDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            // 2. Lấy dữ liệu thô từ Repository
            var rawData = await _attendanceRepository.GetMyAttendanceHistoryAsync(employeeId, startDate, endDate, null);

            // 3. Xử lý gom nhóm (Group By) ngay tại Service
            var dailyData = rawData.GroupBy(a => a.Date.Day)
        .ToDictionary(g => g.Key, g => new DailyAttendanceViewModel
        {
            Date = g.First().Date,
            FirstCheckIn = g.Where(x => x.CheckInTime.HasValue).Min(x => x.CheckInTime),
            LastCheckOut = g.Where(x => x.CheckOutTime.HasValue).Max(x => x.CheckOutTime),
            IsLate = g.Any(x => x.IsLate),
            TotalHours = g.Where(x => x.CheckInTime.HasValue && x.CheckOutTime.HasValue).Any()
                ? (decimal)(g.Max(x => x.CheckOutTime) - g.Min(x => x.CheckInTime)).Value.TotalHours
                : 0,

            // === BỔ SUNG DÒNG NÀY ĐỂ LẤY CHI TIẾT ===
            Details = g.OrderBy(x => x.CheckInTime ?? x.CheckOutTime).ToList()
        });

            return dailyData;
        }
    }
}