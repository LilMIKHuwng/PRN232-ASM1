using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.NewsArticles
{
    [Authorize(Roles = "0,1,2")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public List<NewsArticleDto> Articles { get; set; } = new();

        public async Task OnGetAsync()
        {
            var url = "https://localhost:7015/api/NewsArticles?search=" + Uri.EscapeDataString(Search ?? "");
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<NewsArticleDto>>>(url);

            if (response != null && response.Success)
            {
                Articles = response.Data;
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
