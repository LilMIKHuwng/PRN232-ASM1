using FUNewsManagementWebAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.TagServices;

namespace FUNewsManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _service;

        public TagsController(ITagService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var tags = await _service.GetAllAsync(search);
            return Ok(new ApiResponse<List<TagDto>>(true, "Tag list retrieved successfully.", tags));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tag = await _service.GetByIdAsync(id);
            return tag == null
                ? NotFound(new ApiResponse<string>(false, "Tag not found."))
                : Ok(new ApiResponse<TagDto>(true, "Tag retrieved successfully.", tag));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagCreateDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.TagId },
                new ApiResponse<TagDto>(true, "Tag created successfully.", created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TagUpdateDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            return result
                ? Ok(new ApiResponse<string>(true, "Tag updated successfully."))
                : NotFound(new ApiResponse<string>(false, "Tag not found."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new ApiResponse<string>(true, "Tag deleted successfully."))
                : NotFound(new ApiResponse<string>(false, "Tag not found."));
        }
    }

}
