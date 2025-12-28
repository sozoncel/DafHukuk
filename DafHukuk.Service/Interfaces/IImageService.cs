using Microsoft.AspNetCore.Http;

namespace DafHukuk.Service.Interfaces
{
    public interface IImageService
    {
        Task<(bool Success, string Message, string? FilePath)> UploadImageAsync(
            IFormFile file,
            string entityType,
            int maxWidth = 1920,
            int maxHeight = 1080);

        Task<bool> DeleteImageAsync(string filePath);

        bool ValidateImageFile(IFormFile file, out string errorMessage);
    }
}
