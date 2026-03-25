using System;
using System.ComponentModel.DataAnnotations;

namespace SmartHR_Payroll.ViewModels.Profile
{
    // 2. Dùng làm FORM CHỈNH SỬA (Chỉ cho phép sửa SĐT, Địa chỉ, Ngân hàng)
    public class EditProfileViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = string.Empty;

        public string BankName { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
    }
}