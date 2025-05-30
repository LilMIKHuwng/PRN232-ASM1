using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.Tags
{
    [Authorize(Roles = "0,1")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public List<TagDto> Tags { get; set; } = new();

        public async Task OnGetAsync()
        {
            var url = "https://localhost:7015/api/Tags";
            if (!string.IsNullOrWhiteSpace(Search))
                url += $"?search={Uri.EscapeDataString(Search)}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TagDto>>>(url);
            if (response != null && response.Success)
                Tags = response.Data;
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
