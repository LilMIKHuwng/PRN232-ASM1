using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static FUNewsManagementWebRazorPage.Pages.SystemAccounts.IndexModel;

namespace FUNewsManagementWebRazorPage.Pages.SystemAccounts
{
	[Authorize(Roles = "0")]
	public class NewsStatisticsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public NewsStatisticsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
        public DateTime? StartDate { get; set; }

        [BindProperty]
        public DateTime? EndDate { get; set; }

        public List<NewsArticleDto> Articles { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Nothing special on GET
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                ErrorMessage = "Please provide both Start Date and End Date.";
                return Page();
            }

            if (StartDate > EndDate)
            {
                ErrorMessage = "Start Date cannot be after End Date.";
                return Page();
            }

            try
            {
                var url = $"https://localhost:7015/api/NewsArticles/statistics?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<NewsArticleDto>>>(url);

                if (response != null && response.Success)
                {
                    Articles = response.Data;
                }
                else
                {
                    ErrorMessage = "Failed to load news statistics.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error calling API: {ex.Message}";
            }

            return Page();
        }
    }
}
