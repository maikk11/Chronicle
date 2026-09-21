using Microsoft.AspNetCore.Mvc;
using Chronicle.Services;
using Chronicle.Models.ViewModels;

namespace Chronicle.Controllers;

public class CategoryController : Controller
{
    private readonly ArticleService articleService;
    private readonly CategoryService categoryService;

    public CategoryController(ArticleService articleService, CategoryService categoryService)
    {
        this.articleService = articleService;
        this.categoryService = categoryService;
    }

    [HttpGet]
    [Route("Categories/Search/{categoryIdentifier}")]
    public async Task<IActionResult> Search(string categoryIdentifier)
    {
        var allCategories = await categoryService.ReadAllAsync();

        CategoryDto? category = null;

        if (long.TryParse(categoryIdentifier, out long id))
        {
            category = allCategories.FirstOrDefault(c => c.Id == id);
        }

        if (category == null)
        {
            category = allCategories.FirstOrDefault(c =>
                c.Name.Equals(categoryIdentifier, StringComparison.OrdinalIgnoreCase));
        }

        if (category == null)
        {
            return NotFound();
        }

        var articles = (await articleService.ReadAllAsync())
            .Where(a => a.Category?.Id == category.Id && a.IsAccepted == true)
            .OrderByDescending(a => a.PublishDate ?? a.CreatedAt)
            .ToList();

        ViewBag.Title = $"Articles in the category: {category.Name}";

        return View("~/Views/Article/Index.cshtml", articles);
    }
}