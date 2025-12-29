using DafHukuk.Core.Entities;
using DafHukuk.Data;
using DafHukuk.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DafHukuk.Service
{
    public class SolutionPartnerService : ISolutionPartnerService
    {
        private readonly AppDbContext _context;

        public SolutionPartnerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SolutionPartner>> GetAll()
        {
            return await _context.SolutionPartners
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SolutionPartner?> GetById(int id)
        {
            return await _context.SolutionPartners
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<SolutionPartner> Create(SolutionPartner partner)
        {
            _context.SolutionPartners.Add(partner);
            await _context.SaveChangesAsync();
            return partner;
        }

        public async Task<SolutionPartner?> Update(int id, SolutionPartner partner)
        {
            // ✅ Tracked entity'yi bul
            var existing = await _context.SolutionPartners.FindAsync(id);
            if (existing == null) return null;

            existing.Name = partner.Name;
            existing.ImageUrl = partner.ImageUrl;
            existing.WebsiteUrl = partner.WebsiteUrl;
            existing.IsActive = partner.IsActive;

            existing.Description_TR = partner.Description_TR ?? existing.Description_TR;
            existing.Description_EN = partner.Description_EN;
            existing.Description_AR = partner.Description_AR;

            existing.Slug_TR = partner.Slug_TR ?? existing.Slug_TR;
            existing.Slug_EN = partner.Slug_EN;
            existing.Slug_AR = partner.Slug_AR;

            try
            {
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await _context.SolutionPartners.FindAsync(id);
            if (existing == null) return false;

            _context.SolutionPartners.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}