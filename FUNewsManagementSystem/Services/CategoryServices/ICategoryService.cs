using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllActiveAsync(string? search);
        Task<Category?> GetByIdAsync(short id);
        Task<Category> AddAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(CategoryUpdateDto dto);
        Task<bool> DeleteAsync(short id);
    }
}
