using DafHukuk.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace DafHukuk.Web.Controllers
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageService imageService, ILogger<ImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            [FromForm] IFormFile file,
            [FromQuery] string entityType = "general")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload attempt with no file");
                    return BadRequest(new { success = false, message = "Dosya seçilmedi." });
                }

                _logger.LogInformation($"Upload attempt: {file.FileName}, Size: {file.Length}, Type: {entityType}");

                var result = await _imageService.UploadImageAsync(file, entityType);

                if (!result.Success)
                {
                    _logger.LogWarning($"Upload failed: {result.Message}");
                    return BadRequest(new { success = false, message = result.Message });
                }

                _logger.LogInformation($"Upload successful: {result.FilePath}");

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    filePath = result.FilePath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload exception occurred");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Sunucu hatası: {ex.Message}"
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] string filePath)
        {
            try
            {
                var deleted = await _imageService.DeleteImageAsync(filePath);

                if (!deleted)
                {
                    return NotFound(new { success = false, message = "Dosya bulunamadı." });
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete exception occurred");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}