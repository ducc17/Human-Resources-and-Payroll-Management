using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Models;
using System;
using System.Threading.Tasks;

namespace SmartHR_Payroll.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(DBCodeFirstContext context)
        {
            await context.Database.MigrateAsync();

            // Nếu đã có dữ liệu thì không seed lại
            if (await context.Departments.AnyAsync())
                return;

            const string systemUser = "System";

            // ==========================================
            // 1. DEPARTMENTS (Công ty Tech chỉ cần 5 phòng cốt lõi)
            // ==========================================
            var d1 = new Department { Code = "BOD", Name = "Ban Giám Đốc", CreatedBy = systemUser, UpdatedBy = systemUser };
            var d2 = new Department { Code = "HR", Name = "Hành Chính Nhân Sự", CreatedBy = systemUser, UpdatedBy = systemUser };
            var d3 = new Department { Code = "ENG", Name = "Khối Kỹ Thuật (Engineering)", CreatedBy = systemUser, UpdatedBy = systemUser };
            var d4 = new Department { Code = "PROD", Name = "Khối Sản Phẩm (Product)", CreatedBy = systemUser, UpdatedBy = systemUser };
            var d5 = new Department { Code = "COM", Name = "Khối Kinh Doanh (Commercial)", CreatedBy = systemUser, UpdatedBy = systemUser };

            context.Departments.AddRange(d1, d2, d3, d4, d5);
            await context.SaveChangesAsync();

            // ==========================================
            // 2. POSITIONS (Các vị trí đặc thù ngành IT)
            // ==========================================
            var p1 = new Position { Code = "CEO", Name = "Giám Đốc (CEO)", DepartmentId = d1.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p2 = new Position { Code = "HRM", Name = "Trưởng phòng HCNS", DepartmentId = d2.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p3 = new Position { Code = "TECH_LEAD", Name = "Tech Lead", DepartmentId = d3.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p4 = new Position { Code = "DEV_SENIOR", Name = "Senior Developer", DepartmentId = d3.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p5 = new Position { Code = "DEV_JUNIOR", Name = "Junior Developer", DepartmentId = d3.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p6 = new Position { Code = "QA", Name = "Chuyên viên Kiểm thử (QA)", DepartmentId = d3.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p7 = new Position { Code = "PM", Name = "Product Manager", DepartmentId = d4.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p8 = new Position { Code = "UI_UX", Name = "UI/UX Designer", DepartmentId = d4.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p9 = new Position { Code = "SALES", Name = "IT Sales Executive", DepartmentId = d5.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var p10 = new Position { Code = "HR_EXEC", Name = "Chuyên viên Nhân sự", DepartmentId = d2.DepartmentId, CreatedBy = systemUser, UpdatedBy = systemUser };

            context.Positions.AddRange(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
            await context.SaveChangesAsync();

            // 2.1 Role
            var r1 = new Role { Name = "Admin", Description = "Quản trị hệ thống", CreatedBy = systemUser, UpdatedBy = systemUser };
            var r2 = new Role { Name = "Manager", Description = "Quản lý phòng ban", CreatedBy = systemUser, UpdatedBy = systemUser };
            var r3 = new Role { Name = "Employee", Description = "Nhân viên", CreatedBy = systemUser, UpdatedBy = systemUser };
            var r4 = new Role { Name = "HR", Description = "Nhân sự", CreatedBy = systemUser, UpdatedBy = systemUser };
            context.Role.AddRange(r1, r2, r3, r4);
            await context.SaveChangesAsync();

            // ==========================================
            // 3. EMPLOYEES (10 nhân sự map vào các vị trí trên)
            // ==========================================
            var e1 = new Employee { EmployeeCode = "EMP001", FirstName = "Nguyễn Văn", LastName = "Một", DateOfBirth = new DateOnly(1980, 1, 1), Gender = Status.Gender.Male, Email = "ceo@smarthr.com", PhoneNumber = "0901000001", Address = "Hà Nội", BankAccountNumber = "1000000001", BankName = "Vietcombank", HireDate = new DateOnly(2020, 1, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d1.DepartmentId, PositionId = p1.PositionId, RoleId = r1.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e2 = new Employee { EmployeeCode = "EMP002", FirstName = "Trần Thị", LastName = "Hai", DateOfBirth = new DateOnly(1985, 2, 2), Gender = Status.Gender.Female, Email = "hrm@smarthr.com", PhoneNumber = "0901000002", Address = "Hà Nội", BankAccountNumber = "1000000002", BankName = "BIDV", HireDate = new DateOnly(2020, 2, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d2.DepartmentId, PositionId = p2.PositionId, RoleId = r4.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e3 = new Employee { EmployeeCode = "EMP003", FirstName = "Lê Văn", LastName = "Ba", DateOfBirth = new DateOnly(1990, 3, 3), Gender = Status.Gender.Male, Email = "techlead@smarthr.com", PhoneNumber = "0901000003", Address = "Hà Nội", BankAccountNumber = "1000000003", BankName = "Agribank", HireDate = new DateOnly(2020, 3, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d3.DepartmentId, PositionId = p3.PositionId, RoleId = r2.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e4 = new Employee { EmployeeCode = "EMP004", FirstName = "Phạm Thị", LastName = "Bốn", DateOfBirth = new DateOnly(1992, 4, 4), Gender = Status.Gender.Female, Email = "seniordev@smarthr.com", PhoneNumber = "0901000004", Address = "Đà Nẵng", BankAccountNumber = "1000000004", BankName = "Techcombank", HireDate = new DateOnly(2021, 1, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d3.DepartmentId, PositionId = p4.PositionId, RoleId = r3.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e5 = new Employee { EmployeeCode = "EMP005", FirstName = "Hoàng Văn", LastName = "Năm", DateOfBirth = new DateOnly(1998, 5, 5), Gender = Status.Gender.Male, Email = "juniordev@smarthr.com", PhoneNumber = "0901000005", Address = "Đà Nẵng", BankAccountNumber = "1000000005", BankName = "ACB", HireDate = new DateOnly(2021, 5, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d3.DepartmentId, PositionId = p5.PositionId, RoleId = r3.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e6 = new Employee { EmployeeCode = "EMP006", FirstName = "Vũ Thị", LastName = "Sáu", DateOfBirth = new DateOnly(1996, 6, 6), Gender = Status.Gender.Female, Email = "qa@smarthr.com", PhoneNumber = "0901000006", Address = "Đà Nẵng", BankAccountNumber = "1000000006", BankName = "MBBank", HireDate = new DateOnly(2022, 1, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d3.DepartmentId, PositionId = p6.PositionId, RoleId = r3.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e7 = new Employee { EmployeeCode = "EMP007", FirstName = "Đặng Văn", LastName = "Bảy", DateOfBirth = new DateOnly(1992, 7, 7), Gender = Status.Gender.Male, Email = "pm@smarthr.com", PhoneNumber = "0901000007", Address = "TP.HCM", BankAccountNumber = "1000000007", BankName = "VietinBank", HireDate = new DateOnly(2022, 7, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d4.DepartmentId, PositionId = p7.PositionId, RoleId = r2.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e8 = new Employee { EmployeeCode = "EMP008", FirstName = "Bùi Thị", LastName = "Tám", DateOfBirth = new DateOnly(1998, 8, 8), Gender = Status.Gender.Female, Email = "uiux@smarthr.com", PhoneNumber = "0901000008", Address = "TP.HCM", BankAccountNumber = "1000000008", BankName = "Sacombank", HireDate = new DateOnly(2023, 1, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d4.DepartmentId, PositionId = p8.PositionId, RoleId = r3.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e9 = new Employee { EmployeeCode = "EMP009", FirstName = "Đỗ Văn", LastName = "Chín", DateOfBirth = new DateOnly(1995, 9, 9), Gender = Status.Gender.Male, Email = "sales@smarthr.com", PhoneNumber = "0901000009", Address = "TP.HCM", BankAccountNumber = "1000000009", BankName = "VPBank", HireDate = new DateOnly(2023, 9, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d5.DepartmentId, PositionId = p9.PositionId, RoleId = r3.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };
            var e10 = new Employee { EmployeeCode = "EMP010", FirstName = "Hồ Thị", LastName = "Mười", DateOfBirth = new DateOnly(2000, 10, 10), Gender = Status.Gender.Female, Email = "hrexec@smarthr.com", PhoneNumber = "0901000010", Address = "Cần Thơ", BankAccountNumber = "1000000010", BankName = "TPBank", HireDate = new DateOnly(2024, 1, 1), Status = Status.EmployeeStatus.Active, DepartmentId = d2.DepartmentId, PositionId = p10.PositionId, RoleId = r4.RoleId, CreatedBy = systemUser, UpdatedBy = systemUser };

            context.Employees.AddRange(e1, e2, e3, e4, e5, e6, e7, e8, e9, e10);
            await context.SaveChangesAsync();

            // Cập nhật ManagerId cho Department (Để định tuyến duyệt đơn từ)
            d1.ManagerId = e1.EmployeeId; // CEO quản lý BOD
            d2.ManagerId = e2.EmployeeId; // Trưởng phòng HR
            d3.ManagerId = e3.EmployeeId; // Tech Lead quản lý team Kỹ thuật
            d4.ManagerId = e7.EmployeeId; // PM quản lý khối Product
            d5.ManagerId = e1.EmployeeId; // Tạm thời CEO trực tiếp quản lý Sales

            await context.SaveChangesAsync();

            // ==========================================
            // 4. CONTRACTS
            // ==========================================
            var c1 = new Contract
            {
                EmployeeId = e1.EmployeeId,
                ContractNumber = "HD01",
                Type = Status.ContractType.FullTime,
                StartDate = e1.HireDate,
                EndDate = null,
                BaseSalary = 50000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c2 = new Contract
            {
                EmployeeId = e2.EmployeeId,
                ContractNumber = "HD02",
                Type = Status.ContractType.PartTime,
                StartDate = e2.HireDate,
                EndDate = null,
                BaseSalary = 30000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c3 = new Contract
            {
                EmployeeId = e3.EmployeeId,
                ContractNumber = "HD03",
                Type = Status.ContractType.PartTime,
                StartDate = e3.HireDate,
                EndDate = null,
                BaseSalary = 25000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c4 = new Contract
            {
                EmployeeId = e4.EmployeeId,
                ContractNumber = "HD04",
                Type = Status.ContractType.Probation,
                StartDate = e4.HireDate,
                EndDate = new DateOnly(2026, 12, 31),
                BaseSalary = 40000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c5 = new Contract
            {
                EmployeeId = e5.EmployeeId,
                ContractNumber = "HD05",
                Type = Status.ContractType.Probation,
                StartDate = e5.HireDate,
                EndDate = new DateOnly(2025, 12, 31),
                BaseSalary = 15000000m,
                IsActive = false,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c6 = new Contract
            {
                EmployeeId = e6.EmployeeId,
                ContractNumber = "HD06",
                Type = Status.ContractType.Internship,
                StartDate = e6.HireDate,
                EndDate = new DateOnly(2026, 12, 31),
                BaseSalary = 12000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c7 = new Contract
            {
                EmployeeId = e7.EmployeeId,
                ContractNumber = "HD07",
                Type = Status.ContractType.FullTime,
                StartDate = e7.HireDate,
                EndDate = null,
                BaseSalary = 10000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c8 = new Contract
            {
                EmployeeId = e8.EmployeeId,
                ContractNumber = "HD08",
                Type = Status.ContractType.PartTime,
                StartDate = e8.HireDate,
                EndDate = null,
                BaseSalary = 18000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c9 = new Contract
            {
                EmployeeId = e9.EmployeeId,
                ContractNumber = "HD09",
                Type = Status.ContractType.PartTime,
                StartDate = e9.HireDate,
                EndDate = null,
                BaseSalary = 8000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var c10 = new Contract
            {
                EmployeeId = e10.EmployeeId,
                ContractNumber = "HD10",
                Type = Status.ContractType.Probation,
                StartDate = e10.HireDate,
                EndDate = new DateOnly(2026, 6, 30),
                BaseSalary = 9000000m,
                IsActive = true,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };

            context.Contracts.AddRange(c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);

            // ==========================================
            // 5. LEAVE TYPES
            // ==========================================
            var lt1 = new LeaveType { Name = "Nghỉ phép năm", DefaultDaysPerYear = 12, IsPaidLeave = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt2 = new LeaveType { Name = "Nghỉ ốm hưởng BHXH", DefaultDaysPerYear = 30, IsPaidLeave = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt3 = new LeaveType { Name = "Nghỉ thai sản", DefaultDaysPerYear = 180, IsPaidLeave = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt4 = new LeaveType { Name = "Nghỉ kết hôn", DefaultDaysPerYear = 3, IsPaidLeave = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt5 = new LeaveType { Name = "Nghỉ tang gia", DefaultDaysPerYear = 3, IsPaidLeave = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt6 = new LeaveType { Name = "Nghỉ không lương", DefaultDaysPerYear = 0, IsPaidLeave = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt7 = new LeaveType { Name = "Nghỉ bù", DefaultDaysPerYear = 0, IsPaidLeave = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt8 = new LeaveType { Name = "Khám thai", DefaultDaysPerYear = 5, IsPaidLeave = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt9 = new LeaveType { Name = "Con ốm", DefaultDaysPerYear = 20, IsPaidLeave = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var lt10 = new LeaveType { Name = "Tai nạn lao động", DefaultDaysPerYear = 0, IsPaidLeave = false, CreatedBy = systemUser, UpdatedBy = systemUser };

            context.LeaveTypes.AddRange(lt1, lt2, lt3, lt4, lt5, lt6, lt7, lt8, lt9, lt10);

            // ==========================================
            // 6. ALLOWANCES
            // ==========================================
            var a1 = new Allowance { Name = "Ăn trưa", IsTaxable = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a2 = new Allowance { Name = "Đi lại", IsTaxable = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a3 = new Allowance { Name = "Điện thoại", IsTaxable = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a4 = new Allowance { Name = "Trách nhiệm", IsTaxable = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a5 = new Allowance { Name = "Độc hại", IsTaxable = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a6 = new Allowance { Name = "Nhà ở", IsTaxable = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a7 = new Allowance { Name = "Chuyên cần", IsTaxable = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a8 = new Allowance { Name = "Trang phục", IsTaxable = false, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a9 = new Allowance { Name = "Thâm niên", IsTaxable = true, CreatedBy = systemUser, UpdatedBy = systemUser };
            var a10 = new Allowance { Name = "Nuôi con nhỏ", IsTaxable = false, CreatedBy = systemUser, UpdatedBy = systemUser };

            context.Allowances.AddRange(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);

            // ==========================================
            // 7. DEDUCTIONS
            // ==========================================
            var dd1 = new Deduction { Name = "Bảo hiểm Xã hội (8%)", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd2 = new Deduction { Name = "Bảo hiểm Y tế (1.5%)", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd3 = new Deduction { Name = "Bảo hiểm Thất nghiệp (1%)", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd4 = new Deduction { Name = "Thuế TNCN", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd5 = new Deduction { Name = "Đoàn phí", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd6 = new Deduction { Name = "Phạt đi muộn", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd7 = new Deduction { Name = "Hoàn ứng", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd8 = new Deduction { Name = "Quỹ phòng ban", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd9 = new Deduction { Name = "Bồi thường vật chất", CreatedBy = systemUser, UpdatedBy = systemUser };
            var dd10 = new Deduction { Name = "Khấu trừ khác", CreatedBy = systemUser, UpdatedBy = systemUser };

            context.Deductions.AddRange(dd1, dd2, dd3, dd4, dd5, dd6, dd7, dd8, dd9, dd10);

            await context.SaveChangesAsync();

            // ==========================================
            // 8. PAYROLL PERIODS
            // ==========================================
            var pp1 = new PayrollPeriod
            {
                Name = "Tháng 01/2026",
                StartDate = new DateOnly(2026, 1, 1),
                EndDate = new DateOnly(2026, 1, 31),
                Status = Status.PayrollStatus.Approved,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp2 = new PayrollPeriod
            {
                Name = "Tháng 02/2026",
                StartDate = new DateOnly(2026, 2, 1),
                EndDate = new DateOnly(2026, 2, 28),
                Status = Status.PayrollStatus.Approved,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp3 = new PayrollPeriod
            {
                Name = "Tháng 03/2026",
                StartDate = new DateOnly(2026, 3, 1),
                EndDate = new DateOnly(2026, 3, 31),
                Status = Status.PayrollStatus.Processing,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp4 = new PayrollPeriod
            {
                Name = "Tháng 04/2026",
                StartDate = new DateOnly(2026, 4, 1),
                EndDate = new DateOnly(2026, 4, 30),
                Status = Status.PayrollStatus.Draft,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp5 = new PayrollPeriod
            {
                Name = "Tháng 05/2026",
                StartDate = new DateOnly(2026, 5, 1),
                EndDate = new DateOnly(2026, 5, 31),
                Status = Status.PayrollStatus.Draft,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp6 = new PayrollPeriod
            {
                Name = "Tháng 06/2026",
                StartDate = new DateOnly(2026, 6, 1),
                EndDate = new DateOnly(2026, 6, 30),
                Status = Status.PayrollStatus.Draft,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp7 = new PayrollPeriod
            {
                Name = "Tháng 07/2026",
                StartDate = new DateOnly(2026, 7, 1),
                EndDate = new DateOnly(2026, 7, 31),
                Status = Status.PayrollStatus.Draft,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp8 = new PayrollPeriod
            {
                Name = "Tháng 08/2026",
                StartDate = new DateOnly(2026, 8, 1),
                EndDate = new DateOnly(2026, 8, 31),
                Status = Status.PayrollStatus.Draft,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp9 = new PayrollPeriod
            {
                Name = "Tháng 09/2026",
                StartDate = new DateOnly(2026, 9, 1),
                EndDate = new DateOnly(2026, 9, 30),
                Status = Status.PayrollStatus.Draft,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var pp10 = new PayrollPeriod
            {
                Name = "Tháng 10/2026",
                StartDate = new DateOnly(2026, 10, 1),
                EndDate = new DateOnly(2026, 10, 31),
                Status = Status.PayrollStatus.Draft,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };

            context.PayrollPeriods.AddRange(pp1, pp2, pp3, pp4, pp5, pp6, pp7, pp8, pp9, pp10);
            await context.SaveChangesAsync();

            // ==========================================
            // 9. EMPLOYEE ALLOWANCES
            // ==========================================
            var ea1 = new EmployeeAllowance { EmployeeId = e1.EmployeeId, AllowanceId = a1.AllowanceId, Amount = 730000m, EffectiveDate = e1.HireDate };
            var ea2 = new EmployeeAllowance { EmployeeId = e2.EmployeeId, AllowanceId = a2.AllowanceId, Amount = 500000m, EffectiveDate = e2.HireDate };
            var ea3 = new EmployeeAllowance { EmployeeId = e3.EmployeeId, AllowanceId = a3.AllowanceId, Amount = 300000m, EffectiveDate = e3.HireDate };
            var ea4 = new EmployeeAllowance { EmployeeId = e4.EmployeeId, AllowanceId = a4.AllowanceId, Amount = 1000000m, EffectiveDate = e4.HireDate };
            var ea5 = new EmployeeAllowance { EmployeeId = e5.EmployeeId, AllowanceId = a5.AllowanceId, Amount = 200000m, EffectiveDate = e5.HireDate };
            var ea6 = new EmployeeAllowance { EmployeeId = e6.EmployeeId, AllowanceId = a6.AllowanceId, Amount = 1500000m, EffectiveDate = e6.HireDate };
            var ea7 = new EmployeeAllowance { EmployeeId = e7.EmployeeId, AllowanceId = a7.AllowanceId, Amount = 400000m, EffectiveDate = e7.HireDate };
            var ea8 = new EmployeeAllowance { EmployeeId = e8.EmployeeId, AllowanceId = a8.AllowanceId, Amount = 250000m, EffectiveDate = e8.HireDate };
            var ea9 = new EmployeeAllowance { EmployeeId = e9.EmployeeId, AllowanceId = a9.AllowanceId, Amount = 800000m, EffectiveDate = e9.HireDate };
            var ea10 = new EmployeeAllowance { EmployeeId = e10.EmployeeId, AllowanceId = a10.AllowanceId, Amount = 100000m, EffectiveDate = e10.HireDate };

            context.EmployeeAllowances.AddRange(ea1, ea2, ea3, ea4, ea5, ea6, ea7, ea8, ea9, ea10);

            // ==========================================
            // 10. EMPLOYEE DEDUCTIONS
            // ==========================================
            var ed1 = new EmployeeDeduction { EmployeeId = e1.EmployeeId, DeductionId = dd1.DeductionId, Amount = 4000000m, EffectiveDate = e1.HireDate };
            var ed2 = new EmployeeDeduction { EmployeeId = e2.EmployeeId, DeductionId = dd2.DeductionId, Amount = 450000m, EffectiveDate = e2.HireDate };
            var ed3 = new EmployeeDeduction { EmployeeId = e3.EmployeeId, DeductionId = dd3.DeductionId, Amount = 250000m, EffectiveDate = e3.HireDate };
            var ed4 = new EmployeeDeduction { EmployeeId = e4.EmployeeId, DeductionId = dd4.DeductionId, Amount = 1500000m, EffectiveDate = e4.HireDate };
            var ed5 = new EmployeeDeduction { EmployeeId = e5.EmployeeId, DeductionId = dd5.DeductionId, Amount = 50000m, EffectiveDate = e5.HireDate };
            var ed6 = new EmployeeDeduction { EmployeeId = e6.EmployeeId, DeductionId = dd6.DeductionId, Amount = 100000m, EffectiveDate = e6.HireDate };
            var ed7 = new EmployeeDeduction { EmployeeId = e7.EmployeeId, DeductionId = dd7.DeductionId, Amount = 500000m, EffectiveDate = e7.HireDate };
            var ed8 = new EmployeeDeduction { EmployeeId = e8.EmployeeId, DeductionId = dd8.DeductionId, Amount = 100000m, EffectiveDate = e8.HireDate };
            var ed9 = new EmployeeDeduction { EmployeeId = e9.EmployeeId, DeductionId = dd9.DeductionId, Amount = 200000m, EffectiveDate = e9.HireDate };
            var ed10 = new EmployeeDeduction { EmployeeId = e10.EmployeeId, DeductionId = dd10.DeductionId, Amount = 50000m, EffectiveDate = e10.HireDate };

            context.EmployeeDeductions.AddRange(ed1, ed2, ed3, ed4, ed5, ed6, ed7, ed8, ed9, ed10);

            // ==========================================
            // 11. LEAVE REQUESTS
            // ==========================================
            var lr1 = new LeaveRequest
            {
                EmployeeId = e1.EmployeeId,
                LeaveTypeId = lt1.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 1),
                EndDate = new DateOnly(2026, 3, 2),
                TotalDays = 2m,
                Reason = "Nghỉ phép cá nhân",
                Status = Status.LeaveStatus.Approved,
                ApprovedById = e1.EmployeeId,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr2 = new LeaveRequest
            {
                EmployeeId = e2.EmployeeId,
                LeaveTypeId = lt2.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 5),
                EndDate = new DateOnly(2026, 3, 5),
                TotalDays = 1m,
                Reason = "Sốt siêu vi",
                Status = Status.LeaveStatus.Approved,
                ApprovedById = e2.EmployeeId,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr3 = new LeaveRequest
            {
                EmployeeId = e3.EmployeeId,
                LeaveTypeId = lt4.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 10),
                EndDate = new DateOnly(2026, 3, 12),
                TotalDays = 3m,
                Reason = "Nghỉ cưới",
                Status = Status.LeaveStatus.Approved,
                ApprovedById = e3.EmployeeId,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr4 = new LeaveRequest
            {
                EmployeeId = e4.EmployeeId,
                LeaveTypeId = lt1.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 15),
                EndDate = new DateOnly(2026, 3, 15),
                TotalDays = 1m,
                Reason = "Việc gia đình",
                Status = Status.LeaveStatus.Pending,
                ApprovedById = null,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr5 = new LeaveRequest
            {
                EmployeeId = e5.EmployeeId,
                LeaveTypeId = lt1.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 20),
                EndDate = new DateOnly(2026, 3, 20),
                TotalDays = 0.5m,
                Reason = "Đi ngân hàng",
                Status = Status.LeaveStatus.Rejected,
                ApprovedById = e5.EmployeeId,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr6 = new LeaveRequest
            {
                EmployeeId = e6.EmployeeId,
                LeaveTypeId = lt8.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 22),
                EndDate = new DateOnly(2026, 3, 22),
                TotalDays = 1m,
                Reason = "Khám thai định kỳ",
                Status = Status.LeaveStatus.Approved,
                ApprovedById = e6.EmployeeId,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr7 = new LeaveRequest
            {
                EmployeeId = e7.EmployeeId,
                LeaveTypeId = lt1.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 25),
                EndDate = new DateOnly(2026, 3, 26),
                TotalDays = 2m,
                Reason = "Về quê",
                Status = Status.LeaveStatus.Pending,
                ApprovedById = null,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr8 = new LeaveRequest
            {
                EmployeeId = e8.EmployeeId,
                LeaveTypeId = lt7.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 27),
                EndDate = new DateOnly(2026, 3, 27),
                TotalDays = 1m,
                Reason = "Nghỉ bù làm cuối tuần",
                Status = Status.LeaveStatus.Approved,
                ApprovedById = e8.EmployeeId,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr9 = new LeaveRequest
            {
                EmployeeId = e9.EmployeeId,
                LeaveTypeId = lt9.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 28),
                EndDate = new DateOnly(2026, 3, 29),
                TotalDays = 2m,
                Reason = "Con ốm nhập viện",
                Status = Status.LeaveStatus.Approved,
                ApprovedById = e9.EmployeeId,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var lr10 = new LeaveRequest
            {
                EmployeeId = e10.EmployeeId,
                LeaveTypeId = lt6.LeaveTypeId,
                StartDate = new DateOnly(2026, 3, 30),
                EndDate = new DateOnly(2026, 3, 31),
                TotalDays = 2m,
                Reason = "Việc riêng không lương",
                Status = Status.LeaveStatus.Pending,
                ApprovedById = null,
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };

            context.LeaveRequests.AddRange(lr1, lr2, lr3, lr4, lr5, lr6, lr7, lr8, lr9, lr10);

            // ==========================================
            // 12. ATTENDANCES
            // ==========================================
            var att1 = new Attendance
            {
                EmployeeId = e1.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 0, 0),
                CheckOutTime = new TimeSpan(17, 0, 0),
                TotalHours = 8m,
                IsLate = false,
                Note = "Đúng giờ"
            };
            var att2 = new Attendance
            {
                EmployeeId = e2.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 5, 0),
                CheckOutTime = new TimeSpan(17, 0, 0),
                TotalHours = 7.9m,
                IsLate = true,
                Note = "Đi muộn 5 phút"
            };
            var att3 = new Attendance
            {
                EmployeeId = e3.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(7, 50, 0),
                CheckOutTime = new TimeSpan(17, 30, 0),
                TotalHours = 8.5m,
                IsLate = false,
                Note = "Làm thêm"
            };
            var att4 = new Attendance
            {
                EmployeeId = e4.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 0, 0),
                CheckOutTime = new TimeSpan(16, 0, 0),
                TotalHours = 7m,
                IsLate = false,
                Note = "Ra sớm"
            };
            var att5 = new Attendance
            {
                EmployeeId = e5.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 15, 0),
                CheckOutTime = new TimeSpan(17, 0, 0),
                TotalHours = 7.75m,
                IsLate = true,
                Note = "Đi muộn 15 phút"
            };
            var att6 = new Attendance
            {
                EmployeeId = e6.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(7, 55, 0),
                CheckOutTime = new TimeSpan(17, 0, 0),
                TotalHours = 8m,
                IsLate = false,
                Note = "Bình thường"
            };
            var att7 = new Attendance
            {
                EmployeeId = e7.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 0, 0),
                CheckOutTime = new TimeSpan(18, 0, 0),
                TotalHours = 9m,
                IsLate = false,
                Note = "Tăng ca"
            };
            var att8 = new Attendance
            {
                EmployeeId = e8.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 2, 0),
                CheckOutTime = new TimeSpan(17, 5, 0),
                TotalHours = 8m,
                IsLate = false,
                Note = "Bình thường"
            };
            var att9 = new Attendance
            {
                EmployeeId = e9.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 0, 0),
                CheckOutTime = new TimeSpan(12, 0, 0),
                TotalHours = 4m,
                IsLate = false,
                Note = "Nửa ngày"
            };
            var att10 = new Attendance
            {
                EmployeeId = e10.EmployeeId,
                Date = new DateOnly(2026, 3, 24),
                CheckInTime = new TimeSpan(8, 30, 0),
                CheckOutTime = new TimeSpan(17, 30, 0),
                TotalHours = 8m,
                IsLate = true,
                Note = "Đi muộn 30 phút"
            };

            context.Attendances.AddRange(att1, att2, att3, att4, att5, att6, att7, att8, att9, att10);

            // ==========================================
            // 13. PAYSLIPS
            // ==========================================
            var ps1 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e1.EmployeeId,
                WorkingDays = 22m,
                PaidLeaveDays = 0m,
                BaseSalary = 50000000m,
                TotalAllowances = 730000m,
                TotalDeductions = 4000000m,
                NetSalary = 46730000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps2 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e2.EmployeeId,
                WorkingDays = 21m,
                PaidLeaveDays = 1m,
                BaseSalary = 30000000m,
                TotalAllowances = 500000m,
                TotalDeductions = 450000m,
                NetSalary = 30050000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps3 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e3.EmployeeId,
                WorkingDays = 22m,
                PaidLeaveDays = 0m,
                BaseSalary = 25000000m,
                TotalAllowances = 300000m,
                TotalDeductions = 250000m,
                NetSalary = 25050000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps4 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e4.EmployeeId,
                WorkingDays = 22m,
                PaidLeaveDays = 0m,
                BaseSalary = 40000000m,
                TotalAllowances = 1000000m,
                TotalDeductions = 1500000m,
                NetSalary = 39500000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps5 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e5.EmployeeId,
                WorkingDays = 20m,
                PaidLeaveDays = 2m,
                BaseSalary = 15000000m,
                TotalAllowances = 200000m,
                TotalDeductions = 50000m,
                NetSalary = 15150000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Dữ liệu tham khảo",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps6 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e6.EmployeeId,
                WorkingDays = 22m,
                PaidLeaveDays = 0m,
                BaseSalary = 12000000m,
                TotalAllowances = 1500000m,
                TotalDeductions = 100000m,
                NetSalary = 13400000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps7 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e7.EmployeeId,
                WorkingDays = 22m,
                PaidLeaveDays = 0m,
                BaseSalary = 10000000m,
                TotalAllowances = 400000m,
                TotalDeductions = 500000m,
                NetSalary = 9900000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps8 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e8.EmployeeId,
                WorkingDays = 21.5m,
                PaidLeaveDays = 0.5m,
                BaseSalary = 18000000m,
                TotalAllowances = 250000m,
                TotalDeductions = 100000m,
                NetSalary = 18150000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps9 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e9.EmployeeId,
                WorkingDays = 22m,
                PaidLeaveDays = 0m,
                BaseSalary = 8000000m,
                TotalAllowances = 800000m,
                TotalDeductions = 200000m,
                NetSalary = 8600000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };
            var ps10 = new Payslip
            {
                PayrollPeriodId = pp1.PayrollPeriodId,
                EmployeeId = e10.EmployeeId,
                WorkingDays = 22m,
                PaidLeaveDays = 0m,
                BaseSalary = 9000000m,
                TotalAllowances = 100000m,
                TotalDeductions = 50000m,
                NetSalary = 9050000m,
                PaymentDate = new DateTime(2026, 1, 31),
                Remarks = "Đã chốt lương",
                CreatedBy = systemUser,
                UpdatedBy = systemUser
            };

            context.Payslips.AddRange(ps1, ps2, ps3, ps4, ps5, ps6, ps7, ps8, ps9, ps10);

            await context.SaveChangesAsync();
        }
    }
}