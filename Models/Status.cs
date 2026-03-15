namespace SmartHR_Payroll.Models
{
    public class Status
    {
        public enum Gender { Male = 1, Female = 2, Other = 3 }
        public enum EmployeeStatus { Active = 1, OnLeave = 2, Terminated = 3 }
        public enum ContractType { FullTime = 1, PartTime = 2, Probation = 3, Internship = 4 }
        public enum LeaveStatus { Pending = 1, Approved = 2, Rejected = 3, Cancelled = 4 }
        public enum PayrollStatus { Draft = 1, Processing = 2, Approved = 3, Paid = 4 }
    }
}
