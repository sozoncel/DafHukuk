using DafHukuk.Core.Entities;
using DafHukuk.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
[Route("api/admin/[controller]")]
public class AdminPostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<AdminPostController> _logger;
    private readonly ICategoryService _categoryService;

    public AdminPostController(IPostService postService, ILogger<AdminPostController> logger, ICategoryService categoryService)
    {
        _postService = postService;
        _logger = logger;
        _categoryService = categoryService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePost([FromBody] Post newPost)
    {
        if (newPost == null)
        {
            return BadRequest("Gönderilen veri boş olamaz.");
        }

        try
        {
            if (string.IsNullOrWhiteSpace(newPost.Title_TR))
            {
                return BadRequest("Türkçe başlık (Title_TR) zorunludur.");
            }
            if (newPost.CategoryId == 0)
            {
                return BadRequest("Kategori seçimi (CategoryId) zorunludur.");
            }

            var existingCategory = await _categoryService.GetById(newPost.CategoryId);
            if (existingCategory == null)
            {
                return BadRequest($"Geçersiz Kategori ID'si: {newPost.CategoryId}");
            }

            newPost.CreatedDate = DateTime.UtcNow;
            newPost.IsActive = true;
            newPost.PublishedDate = newPost.PublishedDate == default ? DateTime.UtcNow : newPost.PublishedDate;

            newPost.CoverImageUrl = newPost.CoverImageUrl?.Trim() ?? string.Empty;
            newPost.Title_TR = newPost.Title_TR.Trim();
            newPost.Content_TR = newPost.Content_TR ?? string.Empty;
            newPost.Slug_TR = newPost.Slug_TR?.Trim() ?? string.Empty;
            newPost.Title_EN = newPost.Title_EN?.Trim() ?? string.Empty;
            newPost.Content_EN = newPost.Content_EN ?? string.Empty;
            newPost.Slug_EN = newPost.Slug_EN?.Trim() ?? string.Empty;
            newPost.Title_AR = newPost.Title_AR?.Trim() ?? string.Empty;
            newPost.Content_AR = newPost.Content_AR ?? string.Empty;
            newPost.Slug_AR = newPost.Slug_AR?.Trim() ?? string.Empty;

            var savedPost = await _postService.Create(newPost);

            if (savedPost == null || savedPost.Id == 0)
            {
                return StatusCode(500, "Kayıt işlemi başarısız.");
            }

            return Ok(savedPost);
        }
        catch (DbUpdateException dbEx)
        {
            var inner = dbEx.InnerException?.InnerException?.Message ?? dbEx.InnerException?.Message ?? dbEx.Message;
            _logger.LogError(dbEx, "DbUpdateException: {InnerMessage}", inner);
            return StatusCode(500, $"Veritabanı hatası: {inner}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen hata: {Message}", ex.Message);
            return StatusCode(500, $"Beklenmeyen hata: {ex.Message}");
        }
    }
}