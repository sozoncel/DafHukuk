using DafHukuk.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DafHukuk.Web.Controllers
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            [FromForm] IFormFile file,
            [FromQuery] string entityType = "general")
        {
            if (file == null)
                return BadRequest(new { success = false, message = "Dosya seçilmedi." });

            var result = await _imageService.UploadImageAsync(file, entityType);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new
            {
                success = true,
                message = result.Message,
                filePath = result.FilePath
            });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] string filePath)
        {
            var deleted = await _imageService.DeleteImageAsync(filePath);

            if (!deleted)
                return NotFound(new { success = false, message = "Dosya bulunamadı." });

            return Ok(new { success = true });
        }
    }
}
