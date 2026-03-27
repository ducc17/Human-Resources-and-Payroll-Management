using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using SmartHR_Payroll.Models; 
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHR_Payroll.Data
{
    public class DBCodeFirstContext : DbContext
    {
        public DBCodeFirstContext() { }

        public DBCodeFirstContext(DbContextOptions<DBCodeFirstContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Allowance> Allowances { get; set; }
        public DbSet<EmployeeAllowance> EmployeeAllowances { get; set; }
        public DbSet<Deduction> Deductions { get; set; }
        public DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<TaxBracket> TaxBrackets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            optionsBuilder.UseSqlServer(builder.GetConnectionString("DB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");
                entity.HasKey(d => d.DepartmentId);
                entity.Property(d => d.DepartmentId).HasColumnName("department_id").ValueGeneratedOnAdd();
                entity.Property(d => d.Code).HasColumnName("code").IsRequired().HasMaxLength(20);
                entity.Property(d => d.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(d => d.ManagerId).HasColumnName("manager_id");
                MapAuditableProperties(entity);
                entity.HasIndex(d => d.Code).IsUnique();

                entity.HasOne(d => d.Manager).WithMany().HasForeignKey(d => d.ManagerId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("positions");
                entity.HasKey(p => p.PositionId);
                entity.Property(p => p.PositionId).HasColumnName("position_id").ValueGeneratedOnAdd();
                entity.Property(p => p.Code).HasColumnName("code").IsRequired().HasMaxLength(20);
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                MapAuditableProperties(entity);
                entity.HasIndex(p => p.Code).IsUnique();
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("jobs");
                entity.HasKey(j => j.JobId);
                entity.Property(j => j.JobId).HasColumnName("job_id");
                entity.Property(j => j.DepartmentId).HasColumnName("department_id");
                entity.Property(j => j.PositionId).HasColumnName("position_id");
                MapAuditableProperties(entity);
                entity.HasIndex(j => new { j.DepartmentId, j.PositionId }).IsUnique();

                entity.HasOne(j => j.Department).WithMany(d => d.Jobs).HasForeignKey(j => j.DepartmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(j => j.Position).WithMany(p => p.Jobs).HasForeignKey(j => j.PositionId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");
                entity.HasKey(r => r.RoleId);
                entity.Property(r => r.RoleId).HasColumnName("role_id").ValueGeneratedOnAdd();
                entity.Property(r => r.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
                entity.Property(r => r.Description).HasColumnName("description").HasMaxLength(200);
                MapAuditableProperties(entity);
                entity.HasIndex(r => r.Name).IsUnique();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.EmployeeId).HasColumnName("employee_id").ValueGeneratedOnAdd();
                entity.Property(e => e.EmployeeCode).HasColumnName("employee_code").IsRequired().HasMaxLength(20);
                entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired().HasMaxLength(50);
                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
                entity.Property(e => e.Gender).HasColumnName("gender").IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20);
                entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(500);

                entity.Property(e => e.BankId).HasColumnName("bank_id");
                entity.Property(e => e.BankAccountNumber).HasColumnName("bank_account_number").HasMaxLength(50);
                entity.Property(e => e.DependentCount).HasColumnName("dependent_count").HasDefaultValue(0); 
                entity.Property(e => e.JobId).HasColumnName("job_id").IsRequired();
                entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();

                entity.Property(e => e.HireDate).HasColumnName("hire_date").IsRequired();
                entity.Property(e => e.Status).HasColumnName("status").HasDefaultValue(Status.EmployeeStatus.Active);

                entity.Ignore(e => e.FullName);
                MapAuditableProperties(entity);
                entity.HasIndex(e => e.EmployeeCode).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Job).WithMany(j => j.Employees).HasForeignKey(e => e.JobId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Role).WithMany(r => r.Employees).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Bank).WithMany(b => b.Employees).HasForeignKey(e => e.BankId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("banks");
                entity.HasKey(e => e.BankId);
                entity.Property(e => e.BankCode).HasColumnName("bank_code").HasMaxLength(20).IsRequired();
                entity.Property(e => e.BankName).HasColumnName("bank_name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.ShortName).HasColumnName("short_name").HasMaxLength(50);
                MapAuditableProperties(entity);
            });

            modelBuilder.Entity<Insurance>(entity =>
            {
                entity.ToTable("insurances");
                entity.HasKey(e => e.InsuranceId);
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.EmployeeRate).HasColumnName("employee_rate").HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.CompanyRate).HasColumnName("company_rate").HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.MaxSalaryLimit).HasColumnName("max_salary_limit").HasColumnType("decimal(18,2)");
                MapAuditableProperties(entity);
            });

            modelBuilder.Entity<TaxBracket>(entity =>
            {
                entity.ToTable("tax_brackets");
                entity.HasKey(e => e.TaxBracketId);
                entity.Property(e => e.Level).HasColumnName("level").IsRequired();
                entity.Property(e => e.FromIncome).HasColumnName("from_income").HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ToIncome).HasColumnName("to_income").HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxRate).HasColumnName("tax_rate").HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.QuickSubtraction).HasColumnName("quick_subtraction").HasColumnType("decimal(18,2)").IsRequired();
                MapAuditableProperties(entity);
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("contracts");
                entity.HasKey(c => c.ContractId);

                entity.Property(c => c.ContractId).HasColumnName("contract_id").ValueGeneratedOnAdd();
                entity.Property(c => c.EmployeeId).HasColumnName("employee_id").IsRequired();
                entity.Property(c => c.ContractNumber).HasColumnName("contract_number").IsRequired().HasMaxLength(50);
                entity.Property(c => c.Type).HasColumnName("type").IsRequired();
                entity.Property(c => c.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(c => c.EndDate).HasColumnName("end_date");
                entity.Property(c => c.BaseSalary).HasColumnName("base_salary").HasPrecision(18, 2).IsRequired();
                entity.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);

                MapAuditableProperties(entity);

                entity.HasOne(c => c.Employee)
                      .WithMany(e => e.Contracts)
                      .HasForeignKey(c => c.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("attendances");
                entity.HasKey(a => a.AttendanceId);

                entity.Property(a => a.AttendanceId).HasColumnName("attendance_id").ValueGeneratedOnAdd();
                entity.Property(a => a.EmployeeId).HasColumnName("employee_id").IsRequired();
                entity.Property(a => a.Date).HasColumnName("date").IsRequired();
                entity.Property(a => a.CheckInTime).HasColumnName("check_in_time");
                entity.Property(a => a.CheckOutTime).HasColumnName("check_out_time");
                entity.Property(a => a.TotalHours).HasColumnName("total_hours").HasPrecision(4, 2).HasDefaultValue(0);
                entity.Property(a => a.IsLate).HasColumnName("is_late").HasDefaultValue(false);
                entity.Property(a => a.Note).HasColumnName("note").HasMaxLength(200);

                entity.HasOne(a => a.Employee)
                      .WithMany(e => e.Attendances)
                      .HasForeignKey(a => a.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.ToTable("leave_types");
                entity.HasKey(lt => lt.LeaveTypeId);

                entity.Property(lt => lt.LeaveTypeId).HasColumnName("leave_type_id").ValueGeneratedOnAdd();
                entity.Property(lt => lt.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(lt => lt.DefaultDaysPerYear).HasColumnName("default_days_per_year").IsRequired();
                entity.Property(lt => lt.IsPaidLeave).HasColumnName("is_paid_leave").IsRequired();

                MapAuditableProperties(entity);
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.ToTable("leave_requests");
                entity.HasKey(lr => lr.LeaveRequestId);

                entity.Property(lr => lr.LeaveRequestId).HasColumnName("leave_request_id").ValueGeneratedOnAdd();
                entity.Property(lr => lr.EmployeeId).HasColumnName("employee_id").IsRequired();
                entity.Property(lr => lr.LeaveTypeId).HasColumnName("leave_type_id").IsRequired();
                entity.Property(lr => lr.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(lr => lr.EndDate).HasColumnName("end_date").IsRequired();
                entity.Property(lr => lr.TotalDays).HasColumnName("total_days").HasPrecision(4, 1).IsRequired();
                entity.Property(lr => lr.Reason).HasColumnName("reason").IsRequired().HasMaxLength(500);
                entity.Property(lr => lr.Status).HasColumnName("status").HasDefaultValue(Status.LeaveStatus.Pending);
                entity.Property(lr => lr.ApprovedById).HasColumnName("approved_by_id");

                MapAuditableProperties(entity);

                entity.HasOne(lr => lr.Employee)
                      .WithMany(e => e.LeaveRequests)
                      .HasForeignKey(lr => lr.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.LeaveType)
                      .WithMany()
                      .HasForeignKey(lr => lr.LeaveTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Allowance>(entity =>
            {
                entity.ToTable("allowances");
                entity.HasKey(a => a.AllowanceId);

                entity.Property(a => a.AllowanceId).HasColumnName("allowance_id").ValueGeneratedOnAdd();
                entity.Property(a => a.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(a => a.IsTaxable).HasColumnName("is_taxable").HasDefaultValue(false);

                MapAuditableProperties(entity);
            });

            modelBuilder.Entity<EmployeeAllowance>(entity =>
            {
                entity.ToTable("employee_allowances");
                entity.HasKey(ea => ea.Id);

                entity.Property(ea => ea.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(ea => ea.EmployeeId).HasColumnName("employee_id").IsRequired();
                entity.Property(ea => ea.AllowanceId).HasColumnName("allowance_id").IsRequired();
                entity.Property(ea => ea.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
                entity.Property(ea => ea.EffectiveDate).HasColumnName("effective_date").IsRequired();

                entity.HasOne(ea => ea.Employee)
                      .WithMany(e => e.Allowances)
                      .HasForeignKey(ea => ea.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ea => ea.Allowance)
                      .WithMany()
                      .HasForeignKey(ea => ea.AllowanceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Deduction>(entity =>
            {
                entity.ToTable("deductions");
                entity.HasKey(d => d.DeductionId);

                entity.Property(d => d.DeductionId).HasColumnName("deduction_id").ValueGeneratedOnAdd();
                entity.Property(d => d.Name).HasColumnName("name").IsRequired().HasMaxLength(100);

                MapAuditableProperties(entity);
            });

            modelBuilder.Entity<EmployeeDeduction>(entity =>
            {
                entity.ToTable("employee_deductions");
                entity.HasKey(ed => ed.Id);

                entity.Property(ed => ed.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(ed => ed.EmployeeId).HasColumnName("employee_id").IsRequired();
                entity.Property(ed => ed.DeductionId).HasColumnName("deduction_id").IsRequired();
                entity.Property(ed => ed.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
                entity.Property(ed => ed.EffectiveDate).HasColumnName("effective_date").IsRequired();

                entity.HasOne(ed => ed.Employee)
                      .WithMany(e => e.Deductions)
                      .HasForeignKey(ed => ed.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ed => ed.Deduction)
                      .WithMany()
                      .HasForeignKey(ed => ed.DeductionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PayrollPeriod>(entity =>
            {
                entity.ToTable("payroll_periods");
                entity.HasKey(p => p.PayrollPeriodId);

                entity.Property(p => p.PayrollPeriodId).HasColumnName("payroll_period_id").ValueGeneratedOnAdd();
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(p => p.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(p => p.EndDate).HasColumnName("end_date").IsRequired();
                entity.Property(p => p.Status).HasColumnName("status").HasDefaultValue(Status.PayrollStatus.Draft);

                MapAuditableProperties(entity);
            });

            modelBuilder.Entity<Payslip>(entity =>
            {
                entity.ToTable("payslips");
                entity.HasKey(p => p.PayslipId);
                entity.Property(p => p.PayslipId).HasColumnName("payslip_id").ValueGeneratedOnAdd();
                entity.Property(p => p.PayrollPeriodId).HasColumnName("payroll_period_id").IsRequired();
                entity.Property(p => p.EmployeeId).HasColumnName("employee_id").IsRequired();
                entity.Property(p => p.WorkingDays).HasColumnName("working_days").HasPrecision(4, 1).IsRequired();
                entity.Property(p => p.PaidLeaveDays).HasColumnName("paid_leave_days").HasPrecision(4, 1).IsRequired();
                entity.Property(p => p.BaseSalary).HasColumnName("base_salary").HasPrecision(18, 2).IsRequired();
                entity.Property(p => p.TotalAllowances).HasColumnName("total_allowances").HasPrecision(18, 2).IsRequired();

                entity.Property(p => p.SocialInsuranceAmount).HasColumnName("social_insurance_amount").HasPrecision(18, 2).IsRequired();
                entity.Property(p => p.TaxAmount).HasColumnName("tax_amount").HasPrecision(18, 2).IsRequired();
                entity.Property(p => p.OtherDeductions).HasColumnName("other_deductions").HasPrecision(18, 2).IsRequired();

                entity.Ignore(p => p.TotalDeductions);

                entity.Property(p => p.NetSalary).HasColumnName("net_salary").HasPrecision(18, 2).IsRequired();
                entity.Property(p => p.PaymentDate).HasColumnName("payment_date");
                entity.Property(p => p.Remarks).HasColumnName("remarks").HasMaxLength(200);

                MapAuditableProperties(entity);
                entity.HasOne(p => p.Employee).WithMany(e => e.Payslips).HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.PayrollPeriod).WithMany(pp => pp.Payslips).HasForeignKey(p => p.PayrollPeriodId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Position>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Job>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Contract>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<LeaveType>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<LeaveRequest>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Allowance>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Deduction>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<PayrollPeriod>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Payslip>().HasQueryFilter(x => !x.IsDeleted);
        }

        private void MapAuditableProperties<T>(EntityTypeBuilder<T> entity) where T : AuditableEntity
        {
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is AuditableEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (AuditableEntity)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else
                {
                    Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}