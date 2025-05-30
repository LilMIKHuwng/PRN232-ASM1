using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FUNewsManagementWebRazorPage.Pages.NewsArticles
{
    [Authorize(Roles = "0")]
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [BindProperty]
        public NewsArticleCreateDto Article { get; set; } = new();

        public List<SelectListItem> Tags { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadTagsAsync();
            await LoadCategoriesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadTagsAsync();
                await LoadCategoriesAsync();
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7015/api/NewsArticles", Article);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create article.");
            await LoadTagsAsync();
            await LoadCategoriesAsync();
            return Page();
        }

        private async Task LoadTagsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TagDto>>>("https://localhost:7015/api/Tags");
            if (response?.Success == true)
            {
                Tags = response.Data.Select(tag => new SelectListItem
                {
                    Value = tag.TagId.ToString(),
                    Text = tag.TagName
                }).ToList();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("https://localhost:7015/api/Categories");
            if (response?.Success == true)
            {
                Categories = response.Data.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
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
