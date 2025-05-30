using System.Net.Http.Json;
using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.NewsArticles
{
    [Authorize(Roles = "0")]
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
        public NewsArticleUpdateDto Article { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Tags { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var articleResponse = await _httpClient.GetFromJsonAsync<ApiResponse<NewsArticleDto>>($"https://localhost:7015/api/NewsArticles/{id}");
            if (articleResponse == null || !articleResponse.Success || articleResponse.Data == null)
            {
                return NotFound();
            }

            var articleDto = articleResponse.Data;
            Article = new NewsArticleUpdateDto
            {
                NewsArticleId = articleDto.NewsArticleId,
                NewsTitle = articleDto.NewsTitle,
                Headline = articleDto.Headline,
                NewsContent = articleDto.NewsContent,
                NewsSource = articleDto.NewsSource,
                CategoryId = articleDto.CategoryId,
                NewsStatus = articleDto.NewsStatus,
                Tags = articleDto.Tags?.Select(t => t.TagId).ToList() ?? new List<int>(),
                UpdatedById = articleDto.UpdatedById,
            };

            await LoadCategoriesAndTagsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAndTagsAsync();
                return Page();
            }

            var response = await _httpClient.PutAsJsonAsync("https://localhost:7015/api/NewsArticles", Article);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to update article.");
            await LoadCategoriesAndTagsAsync();
            return Page();
        }

        private async Task LoadCategoriesAndTagsAsync()
        {
            var categoryResponse = await _httpClient.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("https://localhost:7015/api/Categories");
            if (categoryResponse?.Data != null)
            {
                Categories = categoryResponse.Data.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
            }

            var tagResponse = await _httpClient.GetFromJsonAsync<ApiResponse<List<TagDto>>>("https://localhost:7015/api/Tags");
            if (tagResponse?.Data != null)
            {
                Tags = tagResponse.Data.Select(t => new SelectListItem
                {
                    Value = t.TagId.ToString(),
                    Text = t.TagName
                }).ToList();
            }
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
