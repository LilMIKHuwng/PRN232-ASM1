using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Repositories.UnitOfWork;
using Services.TagServices;
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

		public async Task<List<NewsArticleDto>> GetAllAsync(string? search)
		{
			var articles = await _unitOfWork.NewsArticles
				.Query()
				.Where(a =>
					string.IsNullOrWhiteSpace(search) ||
					a.NewsTitle.Contains(search) ||
					a.Headline.Contains(search) ||
					a.NewsContent.Contains(search))
				.Select(article => new NewsArticleDto
				{
					NewsArticleId = article.NewsArticleId,
					NewsTitle = article.NewsTitle,
					Headline = article.Headline,
					CreatedDate = article.CreatedDate,
					NewsContent = article.NewsContent,
					NewsSource = article.NewsSource,
					CategoryId = article.CategoryId,
					NewsStatus = article.NewsStatus,
					CreatedById = article.CreatedById,
					UpdatedById = article.UpdatedById,
					ModifiedDate = article.ModifiedDate,
					Tags = article.Tags != null
						? article.Tags.Select(tag => new TagDto
						{
							TagId = tag.TagId,
							TagName = tag.TagName,
							Note = tag.Note
						}).ToList()
						: new List<TagDto>()
				})
				.ToListAsync();

			// ✅ Trả kết quả về nơi gọi
			return articles;
		}


		public async Task<NewsArticleDto?> GetByIdAsync(string id)
		{
			var article = await _unitOfWork.NewsArticles
				.Query()
				.Include(a => a.Category)
				.Include(a => a.Tags)
				.FirstOrDefaultAsync(a => a.NewsArticleId == id);

			if (article == null) return null;

			return new NewsArticleDto
			{
				NewsArticleId = article.NewsArticleId,
				NewsTitle = article.NewsTitle,
				Headline = article.Headline,
				CreatedDate = article.CreatedDate,
				NewsContent = article.NewsContent,
				NewsSource = article.NewsSource,
				CategoryId = article.CategoryId,
				NewsStatus = article.NewsStatus,
				CreatedById = article.CreatedById,
				UpdatedById = article.UpdatedById,
				ModifiedDate = article.ModifiedDate,
				Tags = article.Tags?.Select(tag => new TagDto
				{
					TagId = tag.TagId,
					TagName = tag.TagName,
					Note = tag.Note
				}).ToList() ?? new List<TagDto>()
			};
		}

		public async Task<NewsArticleDto> AddAsync(NewsArticleCreateDto dto)
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

			if (dto.Tags != null && dto.Tags.Any())
			{
				var tags = await _unitOfWork.Tags
					.Query()
					.Where(t => dto.Tags.Contains(t.TagId))
					.ToListAsync();

				foreach (var tag in tags)
				{
					article.Tags.Add(tag);
				}
			}

			var added = await _unitOfWork.NewsArticles.AddAsync(article);

			return new NewsArticleDto
			{
				NewsArticleId = added.NewsArticleId,
				NewsTitle = added.NewsTitle,
				Headline = added.Headline,
				NewsContent = added.NewsContent,
				NewsSource = added.NewsSource,
				CategoryId = added.CategoryId,
				NewsStatus = added.NewsStatus,
				CreatedById = added.CreatedById,
				CreatedDate = added.CreatedDate,
				Tags = added.Tags?.Select(tag => new TagDto
				{
					TagId = tag.TagId,
					TagName = tag.TagName,
					Note = tag.Note
				}).ToList() ?? new List<TagDto>()
			};
		}

		public async Task<NewsArticleDto?> UpdateAsync(NewsArticleUpdateDto dto)
		{
			var existing = await _unitOfWork.NewsArticles
				.Query()
				.Include(a => a.Tags)
				.FirstOrDefaultAsync(a => a.NewsArticleId == dto.NewsArticleId);

			if (existing == null) return null;

			existing.NewsTitle = dto.NewsTitle;
			existing.Headline = dto.Headline;
			existing.NewsContent = dto.NewsContent;
			existing.NewsSource = dto.NewsSource;
			existing.CategoryId = dto.CategoryId;
			existing.NewsStatus = dto.NewsStatus;
			existing.UpdatedById = dto.UpdatedById;
			existing.ModifiedDate = DateTime.UtcNow;

			// 👉 Cập nhật Tags
			if (dto.Tags != null)
			{
				// Lấy các tag từ DB để liên kết
				var newTags = await _unitOfWork.Tags
					.Query()
					.Where(t => dto.Tags.Contains(t.TagId))
					.ToListAsync();

				// Xóa các tag hiện tại
				existing.Tags.Clear();

				// Gắn lại tag mới (đã lấy từ DB)
				foreach (var tag in newTags)
				{
					existing.Tags.Add(tag);
				}
			}

			await _unitOfWork.NewsArticles.UpdateAsync(existing);

			return new NewsArticleDto
			{
				NewsArticleId = existing.NewsArticleId,
				NewsTitle = existing.NewsTitle,
				Headline = existing.Headline,
				NewsContent = existing.NewsContent,
				NewsSource = existing.NewsSource,
				CategoryId = existing.CategoryId,
				NewsStatus = existing.NewsStatus,
				CreatedById = existing.CreatedById,
				UpdatedById = existing.UpdatedById,
				CreatedDate = existing.CreatedDate,
				ModifiedDate = existing.ModifiedDate,
				Tags = existing.Tags?.Select(tag => new TagDto
				{
					TagId = tag.TagId,
					TagName = tag.TagName,
					Note = tag.Note
				}).ToList() ?? new List<TagDto>()
			};
		}

		public async Task<bool> DeleteAsync(string id)
		{
			var existing = await _unitOfWork.NewsArticles
				.Query()
				.Include(a => a.Tags)
				.FirstOrDefaultAsync(a => a.NewsArticleId == id);

			if (existing == null) return false;

			// 👇 Xóa các liên kết many-to-many trước
			existing.Tags.Clear();

			// 👇 Cập nhật DB để lưu thay đổi bảng trung gian
			await _unitOfWork.SaveChangesAsync();

			// 👇 Bây giờ có thể xóa NewsArticle
			await _unitOfWork.NewsArticles.DeleteAsync(existing);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}


		public async Task<List<NewsArticleDto>> GetStatisticsByPeriodAsync(DateTime startDate, DateTime endDate)
		{
			var articles = await _unitOfWork.NewsArticles
				.Query()
				.Include(a => a.Category)
				.Include(a => a.Tags)
				.Where(a => a.CreatedDate.HasValue &&
							a.CreatedDate.Value.Date >= startDate.Date &&
							a.CreatedDate.Value.Date <= endDate.Date)
				.OrderByDescending(a => a.CreatedDate)
				.ToListAsync();

			return articles.Select(article => new NewsArticleDto
			{
				NewsArticleId = article.NewsArticleId,
				NewsTitle = article.NewsTitle,
				Headline = article.Headline,
				CreatedDate = article.CreatedDate,
				NewsContent = article.NewsContent,
				NewsSource = article.NewsSource,
				CategoryId = article.CategoryId,
				NewsStatus = article.NewsStatus,
				CreatedById = article.CreatedById,
				UpdatedById = article.UpdatedById,
				ModifiedDate = article.ModifiedDate,
				Tags = article.Tags?.Select(tag => new TagDto
				{
					TagId = tag.TagId,
					TagName = tag.TagName,
					Note = tag.Note
				}).ToList() ?? new List<TagDto>()
			}).ToList();
		}


	}



}
