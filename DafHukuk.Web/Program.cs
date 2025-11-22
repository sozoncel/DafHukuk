using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Web.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVISLERIN EKLENMESI ---

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Login yolu ayarý (Önemli: /Admin/Login deðil /Auth/Login yaptýk)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// --- 2. PIPELINE (SIRALAMA ÇOK ÖNEMLÝ) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 1. Önce Rota Sistemi
app.UseRouting();

// 2. Sonra Kimlik ve Yetki
app.UseAuthentication();
app.UseAuthorization();

// 3. En Son Antiforgery (Hatanýn çözümü burasý)
app.UseAntiforgery();

// --- 3. ENDPOINT TANIMLARI ---

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- 4. VERITABANI VE ILK KULLANICI (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Veritabanýný güncelle
    context.Database.Migrate();

    // Admin yoksa oluþtur
    if (!userManager.Users.Any())
    {
        var adminUser = new AppUser
        {
            UserName = "admin@dafhukuk.com",
            Email = "admin@dafhukuk.com",
            EmailConfirmed = true,
            FullName = "Sistem Yöneticisi"
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
    }
}

app.Run();