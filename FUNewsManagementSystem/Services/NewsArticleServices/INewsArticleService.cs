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
		Task<List<NewsArticleDto>> GetAllAsync(string? search);
		Task<NewsArticleDto?> GetByIdAsync(string id);
		Task<NewsArticleDto> AddAsync(NewsArticleCreateDto dto);
		Task<NewsArticleDto?> UpdateAsync(NewsArticleUpdateDto dto);
		Task<bool> DeleteAsync(string id);
	}
}
