namespace DafHukuk.Service.Interfaces
{
    /// <summary>
    /// XSS saldırılarına karşı HTML içeriği temizler
    /// </summary>
    public interface IHtmlSanitizerService
    {
        /// <summary>
        /// Zararlı HTML/JavaScript kodlarını temizler
        /// </summary>
        /// <param name="html">Ham HTML içeriği</param>
        /// <returns>Temizlenmiş güvenli HTML</returns>
        string Sanitize(string? html);
    }
}