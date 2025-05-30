using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.NewsArticles
{
    [Authorize(Roles = "0")]
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [BindProperty]
        public NewsArticleDto Article { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<NewsArticleDto>>($"https://localhost:7015/api/NewsArticles/{id}");

            if (response == null || !response.Success || response.Data == null)
            {
                return NotFound();
            }

            Article = response.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _httpClient.DeleteAsync($"https://localhost:7015/api/NewsArticles/{Article.NewsArticleId}");

            if (result.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to delete the article.");
            return Page();
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
