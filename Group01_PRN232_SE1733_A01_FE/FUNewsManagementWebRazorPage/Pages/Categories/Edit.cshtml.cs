using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUNewsManagementWebRazorPage.Pages.Categories
{
    [Authorize(Roles = "0,1")]
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [BindProperty]
        public CategoryUpdateDto Category { get; set; } = new();

        public List<SelectListItem> ParentCategories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<CategoryDto>>(
                $"https://localhost:7015/api/Categories/{id}");

            if (response?.Success != true || response.Data == null)
            {
                return NotFound();
            }

            var data = response.Data;
            Category = new CategoryUpdateDto
            {
                CategoryId = data.CategoryId,
                CategoryName = data.CategoryName,
                CategoryDesciption = data.CategoryDesciption,
                ParentCategoryId = data.ParentCategoryId,
                IsActive = data.IsActive
            };

            await LoadParentCategoriesAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadParentCategoriesAsync(Category.CategoryId);
                return Page();
            }

            var response = await _httpClient.PutAsJsonAsync("https://localhost:7015/api/Categories", Category);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            await LoadParentCategoriesAsync(Category.CategoryId);
            ModelState.AddModelError(string.Empty, "Update failed.");
            return Page();
        }

        private async Task LoadParentCategoriesAsync(short currentCategoryId)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("https://localhost:7015/api/Categories");
            if (response?.Success == true)
            {
                ParentCategories = response.Data
                    .Where(c => c.CategoryId != currentCategoryId) // prevent self-parenting
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
