using Microsoft.AspNetCore.Mvc;
using Chronicle.Models.Domain;
using Chronicle.Services;
using Microsoft.AspNetCore.Authorization;

namespace Chronicle.Controllers;
public class ArticleController : Controller
{
    private readonly CategoryService categoryService;
    private readonly ArticleService articleService;

    public ArticleController(CategoryService categoryService, ArticleService articleService)
    {
        this.categoryService = categoryService;
        this.articleService = articleService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var articles = (await articleService.ReadAllAsync())
            .Where(a => a.IsAccepted == true)
            .OrderByDescending(a => a.PublishDate ?? a.CreatedAt)
            .ToList();
            ViewBag.Title = "All articles";
            return View(articles);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await categoryService.ReadAllAsync();
        ViewBag.Categories = categories;
        return View(new Article());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Article article, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            await articleService.CreateAsync(article, User, file);
            TempData["SuccessMessage"] = "Article successfully added and submitted for review!!";
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Categories = await categoryService.ReadAllAsync();
        return View(article);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(long id)
    {
        var article = await articleService.ReadAsync(id);
        if(article==null)
        {
            return NotFound();
        }
        return View(article);
    }
}