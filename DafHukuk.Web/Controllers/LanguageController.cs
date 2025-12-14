using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace DafHukuk.Web.Controllers
{
    [Route("language")]
    public class LanguageController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public LanguageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Sayfa URL eşleştirmeleri (TR -> EN -> AR)
        private readonly Dictionary<string, Dictionary<string, string>> _pageRoutes = new()
        {
            [""] = new() { ["tr"] = "/", ["en"] = "/en", ["ar"] = "/ar" },
            ["hakkimizda"] = new() { ["tr"] = "/hakkimizda", ["en"] = "/en/about", ["ar"] = "/ar/about" },
            ["hakkimizda/odullerimiz"] = new() { ["tr"] = "/hakkimizda/odullerimiz", ["en"] = "/en/about/awards", ["ar"] = "/ar/about/awards" },
            ["hakkimizda/sosyal-sorumluluk"] = new() { ["tr"] = "/hakkimizda/sosyal-sorumluluk", ["en"] = "/en/about/social-responsibility", ["ar"] = "/ar/about/social-responsibility" },
            ["hizmetlerimiz"] = new() { ["tr"] = "/hizmetlerimiz", ["en"] = "/en/services", ["ar"] = "/ar/services" },
            ["ekibimiz"] = new() { ["tr"] = "/ekibimiz", ["en"] = "/en/team", ["ar"] = "/ar/team" },
            ["duyurular"] = new() { ["tr"] = "/duyurular", ["en"] = "/en/announcements", ["ar"] = "/ar/announcements" },
            ["yayinlar"] = new() { ["tr"] = "/yayinlar", ["en"] = "/en/publications", ["ar"] = "/ar/publications" },
            ["etkinlikler"] = new() { ["tr"] = "/etkinlikler", ["en"] = "/en/events", ["ar"] = "/ar/events" },
            ["kariyer"] = new() { ["tr"] = "/kariyer", ["en"] = "/en/career", ["ar"] = "/ar/career" },
            ["iletisim"] = new() { ["tr"] = "/iletisim", ["en"] = "/en/contact", ["ar"] = "/ar/contact" },
            ["arama"] = new() { ["tr"] = "/arama", ["en"] = "/en/search", ["ar"] = "/ar/search" },
            ["about"] = new() { ["tr"] = "/hakkimizda", ["en"] = "/en/about", ["ar"] = "/ar/about" },
            ["about/awards"] = new() { ["tr"] = "/hakkimizda/odullerimiz", ["en"] = "/en/about/awards", ["ar"] = "/ar/about/awards" },
            ["about/social-responsibility"] = new() { ["tr"] = "/hakkimizda/sosyal-sorumluluk", ["en"] = "/en/about/social-responsibility", ["ar"] = "/ar/about/social-responsibility" },
            ["services"] = new() { ["tr"] = "/hizmetlerimiz", ["en"] = "/en/services", ["ar"] = "/ar/services" },
            ["team"] = new() { ["tr"] = "/ekibimiz", ["en"] = "/en/team", ["ar"] = "/ar/team" },
            ["announcements"] = new() { ["tr"] = "/duyurular", ["en"] = "/en/announcements", ["ar"] = "/ar/announcements" },
            ["publications"] = new() { ["tr"] = "/yayinlar", ["en"] = "/en/publications", ["ar"] = "/ar/publications" },
            ["events"] = new() { ["tr"] = "/etkinlikler", ["en"] = "/en/events", ["ar"] = "/ar/events" },
            ["career"] = new() { ["tr"] = "/kariyer", ["en"] = "/en/career", ["ar"] = "/ar/career" },
            ["contact"] = new() { ["tr"] = "/iletisim", ["en"] = "/en/contact", ["ar"] = "/ar/contact" },
            ["search"] = new() { ["tr"] = "/arama", ["en"] = "/en/search", ["ar"] = "/ar/search" }
        };

        // Mevcut sayfaların listesi (oluşturulmuş sayfalar)
        private readonly HashSet<string> _existingPages = new()
        {
            "/",
            "/hakkimizda",
            "/hakkimizda/odullerimiz",
            "/hakkimizda/sosyal-sorumluluk",
            "/hizmetlerimiz",
            "/ekibimiz",
            "/duyurular",
            "/yayinlar",
            "/etkinlikler",
            "/kariyer",
            "/iletisim",
            "/arama"
            // Yeni sayfa eklediğinizde buraya ekleyin
            // Örnek: "/yeni-sayfa"
        };

        [HttpGet("change")]
        public IActionResult Change(string lang, string? returnUrl = null)
        {
            // Dil kontrolü
            if (lang != "tr" && lang != "en" && lang != "ar")
                lang = "tr";

            // Cookie'ye kaydet (1 yıl geçerli)
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false,
                Secure = false, // Production'da true yapın
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
            Response.Cookies.Append("user_language", lang, cookieOptions);

            // Eğer returnUrl verilmemişse mevcut sayfayı al
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Request.Headers["Referer"].ToString();
                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = "/";
                }
                else
                {
                    var uri = new Uri(returnUrl);
                    // ✅ DÜZELTME: Query string'i de ekle
                    returnUrl = uri.AbsolutePath + uri.Query;
                }
            }

            // Mevcut sayfayı tespit et ve yeni dildeki karşılığını bul
            var localizedUrl = GetLocalizedUrl(returnUrl, lang);

            // Sayfa var mı kontrol et
            if (!IsPageExists(localizedUrl, lang))
            {
                // Sayfa yoksa ana sayfaya yönlendir
                localizedUrl = lang == "tr" ? "/" : $"/{lang}";
            }

            return Redirect(localizedUrl);
        }

        private bool IsPageExists(string url, string lang)
        {
            // URL'yi normalize et
            var normalizedUrl = url.TrimStart('/').ToLower();

            // Query string'i kaldır (sadece path kontrolü için)
            if (normalizedUrl.Contains('?'))
            {
                normalizedUrl = normalizedUrl.Split('?')[0];
            }

            // Dil prefix'ini kaldır
            if (normalizedUrl.StartsWith("en/"))
                normalizedUrl = "/" + normalizedUrl.Substring(3);
            else if (normalizedUrl.StartsWith("ar/"))
                normalizedUrl = "/" + normalizedUrl.Substring(3);
            else
                normalizedUrl = "/" + normalizedUrl;

            // Boş ise ana sayfa
            if (normalizedUrl == "/" || normalizedUrl == "")
                return true;

            // Mevcut sayfalar listesinde var mı kontrol et
            return _existingPages.Contains(normalizedUrl);
        }

        private string GetLocalizedUrl(string currentUrl, string targetLang)
        {
            // URL'yi temizle ve normalize et
            currentUrl = currentUrl.TrimStart('/').ToLower().Trim();

            // Query string varsa ayır
            var queryString = "";
            if (currentUrl.Contains('?'))
            {
                var parts = currentUrl.Split('?');
                currentUrl = parts[0];
                queryString = "?" + parts[1];
            }

            // Mevcut dil prefix'ini kaldır
            var currentLang = "tr";
            if (currentUrl.StartsWith("en/"))
            {
                currentUrl = currentUrl.Substring(3);
                currentLang = "en";
            }
            else if (currentUrl.StartsWith("ar/"))
            {
                currentUrl = currentUrl.Substring(3);
                currentLang = "ar";
            }

            // Boş URL (ana sayfa)
            if (string.IsNullOrEmpty(currentUrl))
            {
                currentUrl = "";
            }

            // Sayfa routing tablosunda ara
            foreach (var route in _pageRoutes)
            {
                // Tam eşleşme veya başlangıç eşleşmesi
                if (currentUrl == route.Key || currentUrl.StartsWith(route.Key + "/") || currentUrl.StartsWith(route.Key + "?"))
                {
                    if (route.Value.ContainsKey(targetLang))
                    {
                        var baseUrl = route.Value[targetLang];

                        // Eğer alt sayfa varsa (örn: /hakkimizda/detay)
                        if (currentUrl.Length > route.Key.Length && currentUrl.Contains("/"))
                        {
                            var subPath = currentUrl.Substring(route.Key.Length);
                            return baseUrl + subPath + queryString;
                        }

                        return baseUrl + queryString;
                    }
                }
            }

            if (targetLang == "tr")
            {
                return "/" + currentUrl + queryString;
            }
            else
            {
                return $"/{targetLang}/" + currentUrl + queryString;
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            var lang = GetCurrentLanguage();
            return Ok(new { language = lang });
        }

        private string GetCurrentLanguage()
        {
            // 1. Cookie'den kontrol et
            if (Request.Cookies.TryGetValue("user_language", out var cookieLang))
            {
                if (cookieLang == "tr" || cookieLang == "en" || cookieLang == "ar")
                    return cookieLang;
            }

            // 2. URL'den kontrol et
            var path = Request.Path.Value?.ToLower() ?? "";
            if (path.StartsWith("/en/") || path == "/en") return "en";
            if (path.StartsWith("/ar/") || path == "/ar") return "ar";

            // 3. Varsayılan Türkçe
            return "tr";
        }
    }
}