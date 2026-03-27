namespace SmartHR_Payroll.Models
{
    public class TaxBracket : AuditableEntity
    {
        public int TaxBracketId { get; set; }
        public int Level { get; set; } 

        public decimal FromIncome { get; set; }

        public decimal? ToIncome { get; set; }

        public decimal TaxRate { get; set; }

        public decimal QuickSubtraction { get; set; }
    }
}
