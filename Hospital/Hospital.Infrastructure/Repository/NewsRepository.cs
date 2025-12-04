using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Hospital.Infrastructure.Repository
{
    public class NewsRepository : INewsRepository
    {

        private readonly AppDbContext _dbContext;
        public NewsRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<News> AddAsync(News news)
        {
            _dbContext.News.Add(news);
            await _dbContext.SaveChangesAsync();
            return news;
        }

        public async Task<int> DeleteAsync(News news)
        {
            _dbContext.News.Remove(news);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<News>> GetAllAsync(int branchId)
        {
            return await _dbContext.News
                .AsNoTracking()
                .Where(n => n.BranchId == branchId).ToListAsync();
        }

        public async Task<IEnumerable<News>> GetAllNewsInSystemAsync()
        {
            return await _dbContext.News
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<News?> GetAsync(int id)
        {
            return await _dbContext.News
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NewsId == id);
        }

        public async Task<int> UpdateAsync(News news)
        {
            _dbContext.News.Update(news);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
