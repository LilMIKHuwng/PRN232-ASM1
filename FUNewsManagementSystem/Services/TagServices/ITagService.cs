using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.TagServices
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllAsync(string? search);
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag> AddAsync(TagCreateDto dto);
        Task<bool> UpdateAsync(TagUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
