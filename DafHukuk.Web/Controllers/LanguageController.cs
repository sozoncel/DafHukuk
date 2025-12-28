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

        private readonly Dictionary<string, Dictionary<string, string>> _pageRoutes = new()
        {
            [""] = new() { ["tr"] = "/", ["en"] = "/en", ["ar"] = "/ar" },
            ["hakkimizda"] = new() { ["tr"] = "/hakkimizda", ["en"] = "/en/about", ["ar"] = "/ar/about" },
            ["hakkimizda/odullerimiz"] = new() { ["tr"] = "/hakkimizda/odullerimiz", ["en"] = "/en/about/awards", ["ar"] = "/ar/about/awards" },
            ["hakkimizda/sosyal-sorumluluk"] = new() { ["tr"] = "/hakkimizda/sosyal-sorumluluk", ["en"] = "/en/about/social-responsibility", ["ar"] = "/ar/about/social-responsibility" },
            ["hizmetlerimiz"] = new() { ["tr"] = "/hizmetlerimiz", ["en"] = "/en/services", ["ar"] = "/ar/services" },
            ["ekibimiz"] = new() { ["tr"] = "/ekibimiz", ["en"] = "/en/team", ["ar"] = "/ar/team" },
            ["cozum-ortaklari"] = new() { ["tr"] = "/cozum-ortaklari", ["en"] = "/en/partners", ["ar"] = "/ar/partners" },
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
            ["partners"] = new() { ["tr"] = "/cozum-ortaklari", ["en"] = "/en/partners", ["ar"] = "/ar/partners" },
            ["announcements"] = new() { ["tr"] = "/duyurular", ["en"] = "/en/announcements", ["ar"] = "/ar/announcements" },
            ["publications"] = new() { ["tr"] = "/yayinlar", ["en"] = "/en/publications", ["ar"] = "/ar/publications" },
            ["events"] = new() { ["tr"] = "/etkinlikler", ["en"] = "/en/events", ["ar"] = "/ar/events" },
            ["career"] = new() { ["tr"] = "/kariyer", ["en"] = "/en/career", ["ar"] = "/ar/career" },
            ["contact"] = new() { ["tr"] = "/iletisim", ["en"] = "/en/contact", ["ar"] = "/ar/contact" },
            ["search"] = new() { ["tr"] = "/arama", ["en"] = "/en/search", ["ar"] = "/ar/search" }
        };

        private readonly HashSet<string> _existingPages = new()
        {
            "/",
            "/hakkimizda",
            "/hakkimizda/odullerimiz",
            "/hakkimizda/sosyal-sorumluluk",
            "/hizmetlerimiz",
            "/ekibimiz",
            "/cozum-ortaklari",
            "/duyurular",
            "/yayinlar",
            "/etkinlikler",
            "/kariyer",
            "/iletisim",
            "/arama"
        };

        [HttpGet("change")]
        public IActionResult Change(string lang, string? returnUrl = null)
        {
            if (lang != "tr" && lang != "en" && lang != "ar")
                lang = "tr";

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
            Response.Cookies.Append("user_language", lang, cookieOptions);

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
                    returnUrl = uri.AbsolutePath + uri.Query;
                }
            }

            var localizedUrl = GetLocalizedUrl(returnUrl, lang);

            // ✅ GÜVENLİK FIX: Open Redirect koruması
            if (!Url.IsLocalUrl(localizedUrl))
            {
                localizedUrl = lang == "tr" ? "/" : $"/{lang}";
            }

            if (!IsPageExists(localizedUrl, lang))
            {
                localizedUrl = lang == "tr" ? "/" : $"/{lang}";
            }

            return Redirect(localizedUrl);
        }

        private bool IsPageExists(string url, string lang)
        {
            var normalizedUrl = url.TrimStart('/').ToLower();

            if (normalizedUrl.Contains('?'))
            {
                normalizedUrl = normalizedUrl.Split('?')[0];
            }

            if (normalizedUrl.StartsWith("en/"))
                normalizedUrl = "/" + normalizedUrl.Substring(3);
            else if (normalizedUrl.StartsWith("ar/"))
                normalizedUrl = "/" + normalizedUrl.Substring(3);
            else
                normalizedUrl = "/" + normalizedUrl;

            if (normalizedUrl == "/" || normalizedUrl == "")
                return true;

            return _existingPages.Contains(normalizedUrl);
        }

        private string GetLocalizedUrl(string currentUrl, string targetLang)
        {
            currentUrl = currentUrl.TrimStart('/').ToLower().Trim();

            var queryString = "";
            if (currentUrl.Contains('?'))
            {
                var parts = currentUrl.Split('?');
                currentUrl = parts[0];
                queryString = "?" + parts[1];
            }

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

            if (string.IsNullOrEmpty(currentUrl))
            {
                currentUrl = "";
            }

            foreach (var route in _pageRoutes)
            {
                if (currentUrl == route.Key || currentUrl.StartsWith(route.Key + "/") || currentUrl.StartsWith(route.Key + "?"))
                {
                    if (route.Value.ContainsKey(targetLang))
                    {
                        var baseUrl = route.Value[targetLang];

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
            if (Request.Cookies.TryGetValue("user_language", out var cookieLang))
            {
                if (cookieLang == "tr" || cookieLang == "en" || cookieLang == "ar")
                    return cookieLang;
            }

            var path = Request.Path.Value?.ToLower() ?? "";
            if (path.StartsWith("/en/") || path == "/en") return "en";
            if (path.StartsWith("/ar/") || path == "/ar") return "ar";

            return "tr";
        }
    }
}