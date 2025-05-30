using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using static FUNewsManagementWebRazorPage.Pages.Categories.CreateModel;

namespace FUNewsManagementWebRazorPage.Pages.NewsArticles
{
    [Authorize(Roles = "0")]
    public class DetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public NewsArticleDto Article { get; set; }

        public DetailModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<NewsArticleDto>>($"https://localhost:7015/api/NewsArticles/{id}");

            if (response == null || !response.Success || response.Data == null)
            {
                return NotFound();
            }

            Article = response.Data;
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
