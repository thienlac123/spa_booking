using DoAnSPA.Data;
using DoAnSPA.Helpers;
using DoAnSPA.Models;
using DoAnSPA.Repositories;
using DoAnSPA.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("AI key length = " + (builder.Configuration["AI:ApiKey"]?.Length ?? 0));

// Email config
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddTransient<EmailService>();

builder.Services.AddSession();

// EF + Identity (đã có roles vì bạn dùng AddIdentity<SpaUser, IdentityRole>())
builder.Services.AddDbContext<SpaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<SpaUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<SpaDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


// DI repositories
builder.Services.AddScoped<IKhachHangRepository, EFKhachHangRepository>();
builder.Services.AddScoped<IDichVuRepository, EFDichVuRepository>();
builder.Services.AddScoped<ILichHenRepository, EFLichHenRepository>();
builder.Services.AddScoped<INhanVienRepository, EFNhanVienRepository>();
builder.Services.AddScoped<IPhanHoiRepository, EFPhanHoiRepository>();

// Đăng ký HttpClient và Service
builder.Services.AddHttpClient();



// MoMo payment service

builder.Services.Configure<MoMoOptions>(builder.Configuration.GetSection("MoMo"));
builder.Services.AddHttpClient<MoMoService>();

// VnPay payment service

builder.Services.Configure<VnPayOptions>(builder.Configuration.GetSection("VnPay"));
builder.Services.AddSingleton<VnPayService>();


var app = builder.Build();


/* ====== SEED ROLES: đặt SAU khi build app, TRƯỚC pipeline ====== */
await SeedRolesAsync(app);
/* =============================================================== */

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/trangchu/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

/* 👇 thêm dòng này để cookie auth hoạt động */
app.UseAuthentication();

app.UseAuthorization();

/* Mapping endpoint theo kiểu mới (gọn hơn) */
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=trangchu}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

/* ============== helper seed roles ================== */
static async Task SeedRolesAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "SpaOwner", "Staff", "Customer" };
    foreach (var r in roles)
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));
}
