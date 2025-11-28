using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DafHukuk.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> Create(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> Update(int id, Category category)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null)
                return null;

            existing.Name_TR = category.Name_TR;
            existing.Name_EN = category.Name_EN;
            existing.Name_AR = category.Name_AR;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}