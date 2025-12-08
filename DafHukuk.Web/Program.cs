// Program.cs
using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Service;
using DafHukuk.Service.Interfaces;
using DafHukuk.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVISLERIN EKLENMESI ---

builder.Services.AddHttpContextAccessor(); // Gerekli

// HttpClient'ın BaseAddress'ini appsettings.json'dan okuyarak kurduk (Daha güvenli ve temiz)
// NavigationManager'ı burada kullanmaya gerek kalmadı.
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUri = config["BaseAddress"] ?? builder.Configuration.GetValue<string>("Urls:HttpBase") ?? "https://localhost:7033/";
    return new HttpClient
    {
        BaseAddress = new Uri(baseUri)
    };
});

// Custom Servislerin Tanımlanması
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Döngüsel referans düzeltmesi
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 🛡️ KRİTİK GÜVENLİK DÜZELTMESİ: CORS'u sadece kendi domain'ine kısıtla
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        // SADECE GÜVENDİĞİNİZ ADRESLERE İZİN VERİN (PROD adresi buraya gelmeli)
        // HTTPS'i ve portu doğru yazmak çok KRİTİK!
        corsBuilder.WithOrigins("https://localhost:7033", "http://localhost:5240", "https://prod-domaininiz.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); // Auth çerezleri için zorunlu
    });
});
// ----------------------------------------------------

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

// Login yolu ayarı
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// --- 2. PIPELINE (SIRALAMA ÇOK ÖNEMLİ) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 1. Önce Rota Sistemi
app.UseRouting();

// 🛡️ KRİTİK DÜZELTME 4: CORS'u UseRouting'den hemen sonra etkinleştir.
// Eğer UseRouting yoksa, UseAuthentication'dan önce olmalıdır.
app.UseCors();

// 2. Sonra Kimlik ve Yetki
app.UseAuthentication();
app.UseAuthorization();

// 3. En Son Antiforgery (Bu, Blazor sayfaları için gereklidir)
app.UseAntiforgery();

// --- 3. ENDPOINT TANIMLARI ---

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- 4. VERITABANI VE ILK KULLANICI (SEED DATA) ---
// ... (Mevcut Seeding kodunuzu koruyun) ...

app.Run();