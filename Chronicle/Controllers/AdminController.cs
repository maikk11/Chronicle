using Chronicle.Repositories;
using Chronicle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chronicle.Models.Domain;

namespace Chronicle.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ICareerRequestRepository careerRequestRepository;
    private readonly CategoryService categoryService;

    public AdminController(
        ICareerRequestRepository careerRequestRepository,
        CategoryService categoryService)
    {
        this.careerRequestRepository = careerRequestRepository;
        this.categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var requests = await careerRequestRepository.FindByIsCheckedAsync(false);
        var categories = await categoryService.ReadAllAsync();

        ViewBag.Requests = requests;
        ViewBag.Categories = categories;

        return View();
    }
        [HttpGet]
    public IActionResult CreateCategory()
    {
        return View(new Category());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        await categoryService.CreateAsync(category, User, null);
        TempData["SuccessMessage"] = "Category created.";
        return RedirectToAction("Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(long id)
    {
        var categoryDto = await categoryService.ReadAsync(id);
        if (categoryDto == null)
        {
            return NotFound();
        }

        var category = new Category
        {
            Id = categoryDto.Id,
            Name = categoryDto.Name
        };

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        await categoryService.UpdateAsync(category.Id, category, null);
        TempData["SuccessMessage"] = "Category updated.";
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        await categoryService.DeleteAsync(id);
        TempData["SuccessMessage"] = "Category deleted.";
        return RedirectToAction("Dashboard");
    }
}