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
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();
        }

        public async Task<Post?> GetById(int id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Post> Create(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            post.Category = null;

            return post;
        }

        public async Task<Post?> Update(int id, Post post)
        {
            var existing = await _context.Posts.FindAsync(id);
            if (existing == null)
                return null;

            existing.CoverImageUrl = post.CoverImageUrl;
            existing.PublishedDate = post.PublishedDate;
            existing.CategoryId = post.CategoryId;
            existing.IsActive = post.IsActive;


            // TÜRKÇE ALANLARIN GÜNCELLENMESİ
            // Title_TR her zaman güncellenir (ana dil)
            existing.Title_TR = post.Title_TR;

            // Content_TR ve Slug_TR: Eğer gelen veri boş değilse güncelle
            if (!string.IsNullOrWhiteSpace(post.Content_TR))
                existing.Content_TR = post.Content_TR;

            if (!string.IsNullOrWhiteSpace(post.Slug_TR))
                existing.Slug_TR = post.Slug_TR;

            // İNGİLİZCE ALANLARIN GÜNCELLENMESİ
            // SADECE boş/null değilse güncelle, yoksa mevcut veriyi koru
            if (!string.IsNullOrWhiteSpace(post.Title_EN))
                existing.Title_EN = post.Title_EN;

            if (!string.IsNullOrWhiteSpace(post.Content_EN))
                existing.Content_EN = post.Content_EN;

            if (!string.IsNullOrWhiteSpace(post.Slug_EN))
                existing.Slug_EN = post.Slug_EN;

            // ARAPÇA ALANLARIN GÜNCELLENMESİ
            // SADECE boş/null değilse güncelle, yoksa mevcut veriyi koru
            if (!string.IsNullOrWhiteSpace(post.Title_AR))
                existing.Title_AR = post.Title_AR;

            if (!string.IsNullOrWhiteSpace(post.Content_AR))
                existing.Content_AR = post.Content_AR;

            if (!string.IsNullOrWhiteSpace(post.Slug_AR))
                existing.Slug_AR = post.Slug_AR;

            await _context.SaveChangesAsync();
            return existing;
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
                    (p.Title_TR.ToLower().Contains(lowerQuery) ||
                     p.Content_TR.ToLower().Contains(lowerQuery) ||
                     p.Slug_TR.ToLower().Contains(lowerQuery) ||
                     p.Title_EN.ToLower().Contains(lowerQuery) ||
                     p.Content_EN.ToLower().Contains(lowerQuery) ||
                     p.Slug_EN.ToLower().Contains(lowerQuery) ||
                     p.Title_AR.ToLower().Contains(lowerQuery) ||
                     p.Content_AR.ToLower().Contains(lowerQuery) ||
                     p.Slug_AR.ToLower().Contains(lowerQuery) ||
                     (p.Category != null && p.Category.Name_TR.ToLower().Contains(lowerQuery)) ||
                     (p.Category != null && p.Category.Name_EN.ToLower().Contains(lowerQuery)) ||
                     (p.Category != null && p.Category.Name_AR.ToLower().Contains(lowerQuery)))
                )
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();

            return results;
        }
    }
}