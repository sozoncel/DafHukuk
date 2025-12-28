using DafHukuk.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using System.IO;
using System.Linq;

namespace DafHukuk.Service
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly string[] _allowedFormats = { "JPEG", "PNG", "WEBP" };

        private const long MaxFileSize = 10 * 1024 * 1024;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool ValidateImageFile(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (file == null || file.Length == 0)
            {
                errorMessage = "Dosya boş.";
                return false;
            }

            if (file.Length > MaxFileSize)
            {
                errorMessage = "Dosya boyutu 10MB'dan büyük.";
                return false;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
            {
                errorMessage = $"Geçersiz dosya uzantısı: {ext}";
                return false;
            }

            return true;
        }

        public async Task<(bool Success, string Message, string? FilePath)> UploadImageAsync(
            IFormFile file,
            string entityType,
            int maxWidth = 1920,
            int maxHeight = 1080)
        {
            try
            {
                if (!ValidateImageFile(file, out var validationError))
                    return (false, validationError, null);

                using var sourceStream = file.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await sourceStream.CopyToAsync(memoryStream);

                if (memoryStream.Length == 0)
                    return (false, "Dosya okunamadı.", null);

                memoryStream.Position = 0;

                var format = Image.DetectFormat(memoryStream);
                if (format == null)
                    return (false, "Görsel formatı algılanamadı.", null);

                var formatName = format.Name.ToUpperInvariant();
                if (!_allowedFormats.Contains(formatName))
                    return (false, $"Desteklenmeyen format: {format.Name}", null);

                memoryStream.Position = 0;

                using var image = Image.Load(memoryStream);

                // ✅ DÜZELTME: Uniform aspect ratio (4:5) + Crop mode
                var targetWidth = maxWidth;
                var targetHeight = (int)(maxWidth * 1.25); // 4:5 ratio

                if (image.Width != targetWidth || image.Height != targetHeight)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(targetWidth, targetHeight),
                        Position = AnchorPositionMode.Center
                    }));
                }

                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadDir = Path.Combine(webRoot, "uploads", entityType.ToLowerInvariant());

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = $"{entityType}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.webp";
                var savePath = Path.Combine(uploadDir, fileName);

                await image.SaveAsWebpAsync(savePath);

                var relativePath = $"/uploads/{entityType.ToLowerInvariant()}/{fileName}";
                return (true, "Görsel başarıyla yüklendi.", relativePath);
            }
            catch (Exception ex)
            {
                return (false, $"Yükleme hatası: {ex.Message}", null);
            }
        }

        public async Task<bool> DeleteImageAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;

                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // ✅ GÜVENLİK FIX: Path Traversal koruması
                var sanitizedPath = filePath.TrimStart('/').Replace("..", "");
                var fullPath = Path.GetFullPath(Path.Combine(webRoot, sanitizedPath));

                // WebRoot dışına çıkışı engelle
                if (!fullPath.StartsWith(webRoot, StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Geçersiz dosya yolu.");
                }

                if (!File.Exists(fullPath))
                    return false;

                await Task.Run(() => File.Delete(fullPath));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}