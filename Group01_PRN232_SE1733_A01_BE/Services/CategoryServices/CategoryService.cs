using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private CategoryDto ToDto(Category entity)
            => new CategoryDto
            {
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName,
                CategoryDesciption = entity.CategoryDesciption,
                ParentCategoryId = entity.ParentCategoryId,
                IsActive = entity.IsActive
            };

        private Category ToEntity(CategoryCreateDto dto)
            => new Category
            {
                CategoryName = dto.CategoryName,
                CategoryDesciption = dto.CategoryDesciption,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = dto.IsActive ?? true
            };

        public async Task<List<CategoryDto>> GetAllActiveAsync(string? search)
        {
            var all = await _unitOfWork.Categories.GetAllAsync();
            var filtered = all.Where(c =>
                c.IsActive == true &&
                (string.IsNullOrWhiteSpace(search) ||
                 c.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 c.CategoryDesciption.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return filtered.Select(ToDto).ToList();
        }

        public async Task<CategoryDto?> GetByIdAsync(short id)
        {
            var entity = await _unitOfWork.Categories.GetByIdAsync(id);
            if (entity == null)
                return null;

            return ToDto(entity);
        }

        public async Task<CategoryDto> AddAsync(CategoryCreateDto dto)
        {
            var entity = ToEntity(dto);
            var added = await _unitOfWork.Categories.AddAsync(entity);
            return ToDto(added);
        }

        public async Task<bool> UpdateAsync(CategoryUpdateDto dto)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if (existing == null) return false;

            existing.CategoryName = dto.CategoryName;
            existing.CategoryDesciption = dto.CategoryDesciption;
            existing.ParentCategoryId = dto.ParentCategoryId;
            existing.IsActive = dto.IsActive;

            await _unitOfWork.Categories.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return false;

            var isInUse = await _unitOfWork.NewsArticles
                .Query()
                .AnyAsync(na => na.CategoryId == id);

            if (isInUse)
                return false;

            await _unitOfWork.Categories.DeleteAsync(category);
            return true;
        }
    }


}
