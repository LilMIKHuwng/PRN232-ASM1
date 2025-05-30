using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.Categories
{
    [Authorize(Roles = "0,1")]
    public class DetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        public CategoryDto Category { get; set; } = new();
        public string? ParentCategoryName { get; set; }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<CategoryDto>>(
                $"https://localhost:7015/api/Categories/{id}");

            if (response == null || !response.Success || response.Data == null)
                return NotFound();

            Category = response.Data;

            if (Category.ParentCategoryId.HasValue)
            {
                var parentResponse = await _httpClient.GetFromJsonAsync<ApiResponse<CategoryDto>>(
                    $"https://localhost:7015/api/Categories/{Category.ParentCategoryId.Value}");

                if (parentResponse?.Success == true)
                    ParentCategoryName = parentResponse.Data.CategoryName;
            }

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
