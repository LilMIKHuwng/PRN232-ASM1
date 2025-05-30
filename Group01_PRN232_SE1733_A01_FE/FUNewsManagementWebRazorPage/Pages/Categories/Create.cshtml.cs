using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUNewsManagementWebRazorPage.Pages.Categories
{
    [Authorize(Roles = "0,1")]
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [BindProperty]
        public CategoryCreateDto Category { get; set; } = new();

        public List<SelectListItem> ParentCategories { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadParentCategoriesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadParentCategoriesAsync();
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7015/api/Categories", Category);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            await LoadParentCategoriesAsync();
            ModelState.AddModelError(string.Empty, "Create failed.");
            return Page();
        }

        private async Task LoadParentCategoriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("https://localhost:7015/api/Categories");
            if (response?.Success == true)
            {
                ParentCategories = response.Data
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
            }

            ParentCategories.Insert(0, new SelectListItem { Value = "", Text = "No Parent" });
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
