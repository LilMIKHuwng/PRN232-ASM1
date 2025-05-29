using FUNewsManagementWebAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.NewsArticleServices;

namespace FUNewsManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleService _service;

        public NewsArticlesController(INewsArticleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var articles = await _service.GetAllAsync(search);
            return Ok(new ApiResponse<List<NewsArticleDto>>(true, "Articles retrieved successfully", articles));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var article = await _service.GetByIdAsync(id);
            return article == null
                ? NotFound(new ApiResponse<string>(false, "Article not found"))
                : Ok(new ApiResponse<NewsArticleDto>(true, "Article retrieved", article));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewsArticleCreateDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.NewsArticleId },
                new ApiResponse<NewsArticleDto>(true, "Article created", created));
        }

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] NewsArticleUpdateDto dto)
		{
			var result = await _service.UpdateAsync(dto);

			if (result == null)
			{
				return NotFound(new ApiResponse<string>(false, "Article not found"));
			}

			return Ok(new ApiResponse<NewsArticleDto>(true, "Article updated", result));
		}

		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new ApiResponse<string>(true, "Article deleted"))
                : NotFound(new ApiResponse<string>(false, "Article not found"));
        }
    }

}
