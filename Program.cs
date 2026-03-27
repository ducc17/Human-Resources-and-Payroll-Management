using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using SmartHR_Payroll.Data;
using SmartHR_Payroll.Repositories;
using SmartHR_Payroll.Repositories.IRepositories;
using SmartHR_Payroll.Services;
using SmartHR_Payroll.Services.IServices;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AttendanceService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DBCodeFirstContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register the AuthService for dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
// Configure authentication with cookie and Google external login
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "ApplicationCookie";
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie("ApplicationCookie", options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
})
.AddCookie("ExternalCookie")
.AddGoogle(options =>
{
    options.SignInScheme = "ExternalCookie"; 

    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    options.CallbackPath = "/signin-google";
});
// Suport MVC pattern
builder.Services.AddControllersWithViews();

//  Employee
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// Contract
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();

// Attendance
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

//Profile
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Department
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

//Position
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IPositionService, PositionService>();

//Role
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Tax
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<ITaxService, TaxService>();

// Insurance
builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();

// Payroll Period
builder.Services.AddScoped<IPayrollPeriodRepository, PayrollPeriodRepository>();
builder.Services.AddScoped<IPayrollPeriodService, PayrollPeriodService>();

// Payslip
builder.Services.AddScoped<IPayslipRepository, PayslipRepository>();
builder.Services.AddScoped<IPayslipService, PayslipService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DBCodeFirstContext>();

        
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi trong quá trình khởi tạo và seed dữ liệu vào Database.");
    }
}

app.Run();
