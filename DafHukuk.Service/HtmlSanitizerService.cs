using DafHukuk.Service.Interfaces;
using Ganss.Xss;

namespace DafHukuk.Service
{
    /// <summary>
    /// XSS koruması için HTML temizleme servisi
    /// NuGet: Ganss.Xss (HtmlSanitizer)
    /// </summary>
    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();

            // İzin verilen HTML etiketleri (whitelist yaklaşımı)
            _sanitizer.AllowedTags.Clear();
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("br");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("em");
            _sanitizer.AllowedTags.Add("u");
            _sanitizer.AllowedTags.Add("h1");
            _sanitizer.AllowedTags.Add("h2");
            _sanitizer.AllowedTags.Add("h3");
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("ol");
            _sanitizer.AllowedTags.Add("li");
            _sanitizer.AllowedTags.Add("a");
            _sanitizer.AllowedTags.Add("blockquote");

            // İzin verilen HTML attribute'leri
            _sanitizer.AllowedAttributes.Clear();
            _sanitizer.AllowedAttributes.Add("href");
            _sanitizer.AllowedAttributes.Add("target");
            _sanitizer.AllowedAttributes.Add("rel");

            // JavaScript event handler'larını engelle
            _sanitizer.AllowedSchemes.Clear();
            _sanitizer.AllowedSchemes.Add("http");
            _sanitizer.AllowedSchemes.Add("https");
            _sanitizer.AllowedSchemes.Add("mailto");
        }

        public string Sanitize(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            return _sanitizer.Sanitize(html);
        }
    }
}