using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.TagServices
{
	public class TagService : ITagService
	{
		private readonly IUnitOfWork _unitOfWork;

		public TagService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<TagDto>> GetAllAsync(string? search)
		{
			var tags = await _unitOfWork.Tags.GetAllAsync();

			var filteredTags = tags.Where(t =>
				string.IsNullOrEmpty(search) ||
				t.TagName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
				(t.Note != null && t.Note.Contains(search, StringComparison.OrdinalIgnoreCase)))
				.ToList();

			return filteredTags.Select(t => new TagDto
			{
				TagId = t.TagId,
				TagName = t.TagName,
				Note = t.Note
			}).ToList();
		}

		public async Task<TagDto?> GetByIdAsync(int id)
		{
			var tag = await _unitOfWork.Tags
				.Query()
				.Include(t => t.NewsArticles)
				.FirstOrDefaultAsync(t => t.TagId == id);

			if (tag == null) return null;

			return new TagDto
			{
				TagId = tag.TagId,
				TagName = tag.TagName,
				Note = tag.Note
			};
		}

		public async Task<TagDto> AddAsync(TagCreateDto dto)
		{
			var tag = new Tag
			{
				TagId = dto.TagId,
				TagName = dto.TagName,
				Note = dto.Note
			};

			var added = await _unitOfWork.Tags.AddAsync(tag);

			return new TagDto
			{
				TagId = added.TagId,
				TagName = added.TagName,
				Note = added.Note
			};
		}

		public async Task<bool> UpdateAsync(TagUpdateDto dto)
		{
			var existing = await _unitOfWork.Tags.GetByIdAsync(dto.TagId);
			if (existing == null) return false;

			existing.TagName = dto.TagName;
			existing.Note = dto.Note;

			await _unitOfWork.Tags.UpdateAsync(existing);
			return true;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var tag = await _unitOfWork.Tags
				.Query()
				.Include(t => t.NewsArticles)
				.FirstOrDefaultAsync(t => t.TagId == id);

			if (tag == null) return false;

			// 👇 Xoá liên kết many-to-many với NewsArticles trước
			tag.NewsArticles.Clear();
			await _unitOfWork.SaveChangesAsync();

			// 👇 Xoá tag sau khi xoá liên kết
			await _unitOfWork.Tags.DeleteAsync(tag);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}
	}


}
