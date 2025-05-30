using FUNewsManagementWebAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.CategoryServices;

namespace FUNewsManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var categories = await _service.GetAllActiveAsync(search);
            return Ok(new ApiResponse<List<CategoryDto>>(true, "Category list retrieved successfully.", categories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse<string>(false, "Category not found."));

            return Ok(new ApiResponse<CategoryDto>(true, "Category retrieved successfully.", category));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CategoryId },
                new ApiResponse<CategoryDto>(true, "Category created successfully.", created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            if (!result)
                return NotFound(new ApiResponse<string>(false, "Category not found."));

            return Ok(new ApiResponse<string>(true, "Category updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return BadRequest(new ApiResponse<string>(false, "Cannot delete category. It may be in use or not found."));

            return Ok(new ApiResponse<string>(true, "Category deleted successfully."));
        }
    }
}
