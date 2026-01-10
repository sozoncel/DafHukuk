using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DafHukuk.Service
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetAll(int? categoryId = null)
        {
            var query = _context.Posts
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsNoTracking()
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();
        }

        public async Task<List<Post>> GetByServiceType(ServiceType serviceType)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.ServiceType == serviceType)
                .OrderBy(p => p.Title_TR)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Post?> GetById(int id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post> Create(Post post)
        {
            post.CreatedDate = DateTime.UtcNow;
            if (post.PublishedDate == default)
            {
                post.PublishedDate = DateTime.UtcNow;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return await GetById(post.Id) ?? post;
        }

        public async Task<Post?> Update(int id, Post post)
        {
            var existing = await _context.Posts.FindAsync(id);
            if (existing == null)
                return null;

            existing.CategoryId = post.CategoryId;
            existing.IsActive = post.IsActive;
            existing.PublishedDate = post.PublishedDate;
            existing.CoverImageUrl = post.CoverImageUrl;
            existing.ServiceType = post.ServiceType;

            existing.Title_TR = post.Title_TR;
            existing.Content_TR = post.Content_TR;
            existing.Slug_TR = post.Slug_TR;

            existing.Title_EN = post.Title_EN;
            existing.Content_EN = post.Content_EN;
            existing.Slug_EN = post.Slug_EN;

            existing.Title_AR = post.Title_AR;
            existing.Content_AR = post.Content_AR;
            existing.Slug_AR = post.Slug_AR;

            await _context.SaveChangesAsync();

            return await GetById(id);
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await _context.Posts.FindAsync(id);
            if (existing == null)
                return false;

            _context.Posts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Post>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Post>();

            var lowerQuery = query.ToLower().Trim();

            var results = await _context.Posts
                .Include(p => p.Category)
                .Where(p => p.IsActive &&
                    (
                        (p.Title_TR != null && p.Title_TR.ToLower().Contains(lowerQuery)) ||
                        (p.Content_TR != null && p.Content_TR.ToLower().Contains(lowerQuery)) ||
                        (p.Slug_TR != null && p.Slug_TR.ToLower().Contains(lowerQuery)) ||
                        (p.Title_EN != null && p.Title_EN.ToLower().Contains(lowerQuery)) ||
                        (p.Content_EN != null && p.Content_EN.ToLower().Contains(lowerQuery)) ||
                        (p.Slug_EN != null && p.Slug_EN.ToLower().Contains(lowerQuery)) ||
                        (p.Title_AR != null && p.Title_AR.ToLower().Contains(lowerQuery)) ||
                        (p.Content_AR != null && p.Content_AR.ToLower().Contains(lowerQuery)) ||
                        (p.Slug_AR != null && p.Slug_AR.ToLower().Contains(lowerQuery)) ||
                        (p.Category != null && p.Category.Name_TR != null && p.Category.Name_TR.ToLower().Contains(lowerQuery)) ||
                        (p.Category != null && p.Category.Name_EN != null && p.Category.Name_EN.ToLower().Contains(lowerQuery)) ||
                        (p.Category != null && p.Category.Name_AR != null && p.Category.Name_AR.ToLower().Contains(lowerQuery))
                    )
                )
                .AsNoTracking()
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();

            return results;
        }
    }
}