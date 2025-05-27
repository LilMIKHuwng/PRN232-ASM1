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

        public async Task<List<Tag>> GetAllAsync(string? search)
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();
            return tags.Where(t =>
                string.IsNullOrEmpty(search) ||
                t.TagName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (t.Note != null && t.Note.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public async Task<Tag?> GetByIdAsync(int id)
            => await _unitOfWork.Tags.GetByIdAsync(id);

        public async Task<Tag> AddAsync(TagCreateDto dto)
        {
            var tag = new Tag
            {
                TagId = dto.TagId,
                TagName = dto.TagName,
                Note = dto.Note
            };
            return await _unitOfWork.Tags.AddAsync(tag);
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
            var existing = await _unitOfWork.Tags.GetByIdAsync(id);
            if (existing == null) return false;

            await _unitOfWork.Tags.DeleteAsync(existing);
            return true;
        }
    }

}
