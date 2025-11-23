using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Repository
{
    public class BannerRepository : IBannerRepository
    {

        private readonly AppDbContext _context;
        public BannerRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Banner>> GetAllAsync() => await _context.Banners.ToListAsync();
        public async Task<Banner?> GetByIdAsync(int id) => await _context.Banners.FindAsync(id);

        public async Task<Banner> AddAsync(Banner banner)
        {
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
            return banner;
        }

        public async Task UpdateAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();
            }
        }
    }
}
