using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.NewsArticleServices
{
    public interface INewsArticleService
    {
        Task<List<NewsArticle>> GetAllAsync(string? search);
        Task<NewsArticle?> GetByIdAsync(string id);
        Task<NewsArticle> AddAsync(NewsArticleCreateDto dto);
        Task<bool> UpdateAsync(NewsArticleUpdateDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
