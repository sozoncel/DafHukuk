using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace DafHukuk.Service
{
    public interface ILanguageService
    {
        string GetCurrentLanguage();
        void SetLanguage(string langCode);
        string GetLocalizedUrl(string path, string? targetLang = null);
    }

    public class LanguageService : ILanguageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string LanguageCookieKey = "user_language";

        // Sayfa route mapping'leri
        private readonly Dictionary<string, Dictionary<string, string>> _pageRoutes = new()
        {
            // Ana Sayfa
            [""] = new() { ["tr"] = "/", ["en"] = "/en", ["ar"] = "/ar" },

            // Hakkımızda
            ["hakkimizda"] = new() { ["tr"] = "/hakkimizda", ["en"] = "/en/about", ["ar"] = "/ar/about" },
            ["hakkimizda/odullerimiz"] = new() { ["tr"] = "/hakkimizda/odullerimiz", ["en"] = "/en/about/awards", ["ar"] = "/ar/about/awards" },
            ["hakkimizda/sosyal-sorumluluk"] = new() { ["tr"] = "/hakkimizda/sosyal-sorumluluk", ["en"] = "/en/about/social-responsibility", ["ar"] = "/ar/about/social-responsibility" },

            // Hizmetlerimiz - ANA SAYFA VE ALT SAYFALAR
            ["hizmetlerimiz"] = new() { ["tr"] = "/hizmetlerimiz", ["en"] = "/en/services", ["ar"] = "/ar/services" },
            ["hizmetlerimiz/genel-hizmetler"] = new() { ["tr"] = "/hizmetlerimiz/genel-hizmetler", ["en"] = "/en/services/general-services", ["ar"] = "/ar/services/general-services" },
            ["hizmetlerimiz/ozel-danismanlik"] = new() { ["tr"] = "/hizmetlerimiz/ozel-danismanlik", ["en"] = "/en/services/special-consultancy", ["ar"] = "/ar/services/special-consultancy" },
            ["hizmetlerimiz/uzman-gorusu"] = new() { ["tr"] = "/hizmetlerimiz/uzman-gorusu", ["en"] = "/en/services/expert-opinion", ["ar"] = "/ar/services/expert-opinion" },
            ["hizmetlerimiz/arabuluculuk"] = new() { ["tr"] = "/hizmetlerimiz/arabuluculuk", ["en"] = "/en/services/mediation", ["ar"] = "/ar/services/mediation" },

            // Diğer Sayfalar
            ["ekibimiz"] = new() { ["tr"] = "/ekibimiz", ["en"] = "/en/team", ["ar"] = "/ar/team" },
            ["duyurular"] = new() { ["tr"] = "/duyurular", ["en"] = "/en/announcements", ["ar"] = "/ar/announcements" },
            ["yayinlar"] = new() { ["tr"] = "/yayinlar", ["en"] = "/en/publications", ["ar"] = "/ar/publications" },
            ["etkinlikler"] = new() { ["tr"] = "/etkinlikler", ["en"] = "/en/events", ["ar"] = "/ar/events" },
            ["kariyer"] = new() { ["tr"] = "/kariyer", ["en"] = "/en/career", ["ar"] = "/ar/career" },
            ["iletisim"] = new() { ["tr"] = "/iletisim", ["en"] = "/en/contact", ["ar"] = "/ar/contact" },
            ["arama"] = new() { ["tr"] = "/arama", ["en"] = "/en/search", ["ar"] = "/ar/search" },

            // İngilizce path'lerden Türkçe'ye mapping (geriye uyumluluk)
            ["about"] = new() { ["tr"] = "/hakkimizda", ["en"] = "/en/about", ["ar"] = "/ar/about" },
            ["about/awards"] = new() { ["tr"] = "/hakkimizda/odullerimiz", ["en"] = "/en/about/awards", ["ar"] = "/ar/about/awards" },
            ["about/social-responsibility"] = new() { ["tr"] = "/hakkimizda/sosyal-sorumluluk", ["en"] = "/en/about/social-responsibility", ["ar"] = "/ar/about/social-responsibility" },
            ["services"] = new() { ["tr"] = "/hizmetlerimiz", ["en"] = "/en/services", ["ar"] = "/ar/services" },
            ["services/general-services"] = new() { ["tr"] = "/hizmetlerimiz/genel-hizmetler", ["en"] = "/en/services/general-services", ["ar"] = "/ar/services/general-services" },
            ["services/special-consultancy"] = new() { ["tr"] = "/hizmetlerimiz/ozel-danismanlik", ["en"] = "/en/services/special-consultancy", ["ar"] = "/ar/services/special-consultancy" },
            ["services/expert-opinion"] = new() { ["tr"] = "/hizmetlerimiz/uzman-gorusu", ["en"] = "/en/services/expert-opinion", ["ar"] = "/ar/services/expert-opinion" },
            ["services/mediation"] = new() { ["tr"] = "/hizmetlerimiz/arabuluculuk", ["en"] = "/en/services/mediation", ["ar"] = "/ar/services/mediation" },
            ["team"] = new() { ["tr"] = "/ekibimiz", ["en"] = "/en/team", ["ar"] = "/ar/team" },
            ["announcements"] = new() { ["tr"] = "/duyurular", ["en"] = "/en/announcements", ["ar"] = "/ar/announcements" },
            ["publications"] = new() { ["tr"] = "/yayinlar", ["en"] = "/en/publications", ["ar"] = "/ar/publications" },
            ["events"] = new() { ["tr"] = "/etkinlikler", ["en"] = "/en/events", ["ar"] = "/ar/events" },
            ["career"] = new() { ["tr"] = "/kariyer", ["en"] = "/en/career", ["ar"] = "/ar/career" },
            ["contact"] = new() { ["tr"] = "/iletisim", ["en"] = "/en/contact", ["ar"] = "/ar/contact" },
            ["search"] = new() { ["tr"] = "/arama", ["en"] = "/en/search", ["ar"] = "/ar/search" }
        };

        public LanguageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentLanguage()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "tr";

            // 1. Cookie'den kontrol et
            if (context.Request.Cookies.TryGetValue(LanguageCookieKey, out var cookieLang))
            {
                if (IsValidLanguage(cookieLang))
                    return cookieLang;
            }

            // 2. URL'den kontrol et (ör: /en/about)
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.StartsWith("/en/") || path == "/en") return "en";
            if (path.StartsWith("/ar/") || path == "/ar") return "ar";
            if (path.StartsWith("/tr/") || path == "/tr") return "tr";

            // 3. Varsayılan Türkçe
            return "tr";
        }

        public void SetLanguage(string langCode)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            if (!IsValidLanguage(langCode))
                langCode = "tr";

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            context.Response.Cookies.Append(LanguageCookieKey, langCode, cookieOptions);

            // Culture'ı da ayarla (tarih, sayı formatları için)
            var culture = new CultureInfo(langCode == "ar" ? "ar-SA" : langCode == "en" ? "en-US" : "tr-TR");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public string GetLocalizedUrl(string path, string? targetLang = null)
        {
            var lang = targetLang ?? GetCurrentLanguage();

            // Temizle
            path = path.TrimStart('/').ToLower();

            // Mevcut dil prefix'ini kaldır
            if (path.StartsWith("tr/"))
                path = path.Substring(3);
            else if (path.StartsWith("en/"))
                path = path.Substring(3);
            else if (path.StartsWith("ar/"))
                path = path.Substring(3);

            // Boş path (ana sayfa)
            if (string.IsNullOrEmpty(path))
            {
                return lang == "tr" ? "/" : $"/{lang}";
            }

            // Route mapping'de ara
            if (_pageRoutes.ContainsKey(path) && _pageRoutes[path].ContainsKey(lang))
            {
                return _pageRoutes[path][lang];
            }

            // Eğer route mapping'de yoksa, varsayılan mantığı kullan
            return lang == "tr" ? $"/{path}" : $"/{lang}/{path}";
        }

        private bool IsValidLanguage(string lang)
        {
            return lang == "tr" || lang == "en" || lang == "ar";
        }
    }
}