using System.ComponentModel.DataAnnotations;

namespace SmartHR_Payroll.ViewModels.TaxBracket
{
    public class TaxBracketViewModel
    {
        public int TaxBracketId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Bậc thuế")]
        public int Level { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Thu nhập Từ")]
        public decimal FromIncome { get; set; }

        public decimal? ToIncome { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Thuế suất")]
        [Range(0, 100, ErrorMessage = "Tỷ lệ từ 0 - 100%")]
        public decimal TaxRate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mức trừ lùi")]
        public decimal QuickSubtraction { get; set; }
    }
}
