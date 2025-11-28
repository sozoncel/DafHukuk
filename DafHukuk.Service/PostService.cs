using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DafHukuk.Service
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetAll()
        {
            return await _context.Posts
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Post?> GetById(int id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post> Create(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
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
    }
}
