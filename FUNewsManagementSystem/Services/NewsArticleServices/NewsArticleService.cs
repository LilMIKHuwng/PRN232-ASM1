using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.NewsArticleServices
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<NewsArticle>> GetAllAsync(string? search)
        {
            var query = _unitOfWork.NewsArticles.Query();

            query = query.Include(a => a.Category);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.NewsTitle.Contains(search) ||
                    a.Headline.Contains(search) ||
                    a.NewsContent.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _unitOfWork.NewsArticles
                .Query()
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.NewsArticleId == id);
        }

        public async Task<NewsArticle> AddAsync(NewsArticleCreateDto dto)
        {
            var article = new NewsArticle
            {
                NewsArticleId = dto.NewsArticleId,
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryId = dto.CategoryId,
                NewsStatus = dto.NewsStatus,
                CreatedById = dto.CreatedById,
                CreatedDate = DateTime.UtcNow
            };

            return await _unitOfWork.NewsArticles.AddAsync(article);
        }

        public async Task<bool> UpdateAsync(NewsArticleUpdateDto dto)
        {
            var existing = await _unitOfWork.NewsArticles
                .Query()
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.NewsArticleId == dto.NewsArticleId);
            if (existing == null) return false;

            existing.NewsTitle = dto.NewsTitle;
            existing.Headline = dto.Headline;
            existing.NewsContent = dto.NewsContent;
            existing.NewsSource = dto.NewsSource;
            existing.CategoryId = dto.CategoryId;
            existing.NewsStatus = dto.NewsStatus;
            existing.UpdatedById = dto.UpdatedById;
            existing.ModifiedDate = DateTime.UtcNow;

            await _unitOfWork.NewsArticles.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _unitOfWork.NewsArticles
                .Query()
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.NewsArticleId == id);
            if (existing == null) return false;

            await _unitOfWork.NewsArticles.DeleteAsync(existing);
            return true;
        }
    }


}
