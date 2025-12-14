using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Service;
using DafHukuk.Service.Interfaces;
using DafHukuk.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Text.Json.Serialization;
using System.Globalization;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// BÖLÜM 1: SERVİS TANIMLAMALARI (builder.Services)
// =================================================================

// --- TEMEL VE ORTAM SERVİSLERİ ---
builder.Services.AddHttpContextAccessor();
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

// --- VERİTABANI VE KİMLİK (IDENTITY) SERVİSLERİ ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// --- UYGULAMA SERVİSLERİ (BUSINESS LOGIC) ---
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ILawyerService, LawyerService>(); 


// --- CORE VE API SERVİSLERİ ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        corsBuilder.WithOrigins("https://localhost:7033", "http://localhost:5240")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
    });
});

// --- LOKALİZASYON (DİL) SERVİSLERİ ---
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "tr-TR", "en-US", "ar-SA" };
    options.SetDefaultCulture("tr-TR")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});


var app = builder.Build();

// =================================================================
// BÖLÜM 2: MIDDLEWARE PIPELINE (SIRALAMA KRİTİK!)
// =================================================================

// --- HATA YÖNETİMİ ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// --- TEMEL GÜVENLİK VE DOSYALAR ---
app.UseHttpsRedirection();
app.UseStaticFiles();

// --- ROUTING VE AUTHENTICATION BAŞLANGICI ---
app.UseRouting();
app.UseCors(); // UseRouting'den hemen sonra olmalı.

// Authentication ve Authorization, Routing'den ve CORS'tan sonra olmalı.
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();


// --- LOKALİZASYON MIDDLEWARE'İ ---
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr-TR")
    .AddSupportedCultures("tr-TR", "en-US", "ar-SA")
    .AddSupportedUICultures("tr-TR", "en-US", "ar-SA");

app.UseRequestLocalization(localizationOptions);


// --- CUSTOM DİL YÖNLENDİRME MIDDLEWARE'İ (Kök Dizinde Cookie Kontrolü) ---
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";

    // Sadece kök dizin (/) için kontrol yap
    if (path == "/")
    {
        // Cookie'den dil kontrolü yap - SADECE COOKIE VARSA yönlendir
        if (context.Request.Cookies.TryGetValue("user_language", out string? cookieLang))
        {
            // Cookie değeri TR-TR veya tr-TR ise sadece "tr" alıyoruz.
            var shortLang = cookieLang.Split('-')[0].ToLower();

            // Eğer cookie EN veya AR ise yönlendir, TR ise varsayılan (/) kalacak
            if (shortLang == "en")
            {
                context.Response.Redirect("/en");
                return;
            }
            else if (shortLang == "ar")
            {
                context.Response.Redirect("/ar");
                return;
            }
            // cookieLang == "tr" ise yönlendirme yapma, zaten / Türkçe
        }
        // Cookie YOK ise: Varsayılan Türkçe, yönlendirme yok
    }

    await next();
});


// --- MAPPING (ENDPOINT TANIMLAMALARI) ---

// 1. Controller Routing (MVC/API)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// 2. Razor Components (Blazor)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// 3. 404 Hatası Yönlendirme Middleware'i (En sonda olmalı)
// Tüm rotalar denendikten sonra 404 oluşursa NotFound sayfasına yönlendirir.
app.Use(async (context, next) =>
{
    await next();

    // Eğer sayfa bulunamadıysa (404) ve yanıt başlamadıysa NotFound sayfasına yönlendir
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        // Yönlendirme için Request.Path'i değiştirip, pipeline'ı tekrar yürütme
        context.Request.Path = "/NotFound";
        await next();
    }
});


app.Run();