using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DafHukuk.Service
{
    public class LawyerService : ILawyerService
    {
        private readonly AppDbContext _context;

        public LawyerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Lawyer>> GetAll()
        {
            return await _context.Lawyers
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<Lawyer?> GetById(int id)
        {
            return await _context.Lawyers.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lawyer?> GetBySlug(string slug, string language)
        {
            var lowerSlug = slug.ToLower();
            var query = _context.Lawyers.AsQueryable();

            return await (language.ToLower() switch
            {
                "en" => query.FirstOrDefaultAsync(l => l.Slug_EN != null && l.Slug_EN.ToLower() == lowerSlug),
                "ar" => query.FirstOrDefaultAsync(l => l.Slug_AR != null && l.Slug_AR.ToLower() == lowerSlug),
                _ => query.FirstOrDefaultAsync(l => l.Slug_TR.ToLower() == lowerSlug)
            });
        }

        public async Task<Lawyer> Create(Lawyer lawyer)
        {
            _context.Lawyers.Add(lawyer);
            await _context.SaveChangesAsync();
            return lawyer;
        }

        public async Task<Lawyer?> Update(int id, Lawyer lawyer)
        {
            var existing = await _context.Lawyers.FindAsync(id);
            if (existing == null)
                return null;

            // Alanları Güncelleme
            existing.Name = lawyer.Name;
            existing.Email = lawyer.Email;
            existing.MobilePhone = lawyer.MobilePhone;
            existing.OfficePhone = lawyer.OfficePhone;
            existing.Location = lawyer.Location;
            existing.ImageUrl = lawyer.ImageUrl;
            existing.IsActive = lawyer.IsActive;
            existing.UpdatedDate = DateTime.Now;

            // Çoklu Dil Alanlarını Güncelleme
            existing.Title_TR = lawyer.Title_TR;
            existing.Title_EN = lawyer.Title_EN;
            existing.Title_AR = lawyer.Title_AR;

            existing.Slug_TR = lawyer.Slug_TR;
            existing.Slug_EN = lawyer.Slug_EN;
            existing.Slug_AR = lawyer.Slug_AR;

            existing.Biography_TR = lawyer.Biography_TR;
            existing.Biography_EN = lawyer.Biography_EN;
            existing.Biography_AR = lawyer.Biography_AR;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await _context.Lawyers.FindAsync(id);
            if (existing == null)
                return false;

            _context.Lawyers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}