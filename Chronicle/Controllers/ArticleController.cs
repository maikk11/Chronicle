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
    public async Task<IActionResult> Index()
    {
        var articles = await articleService.ReadAllAsync();
        ViewBag.Title = "Tutti gli articoli";
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
            TempData["SuccessMessage"] = "Articolo aggiunto con successo ed inviato in revisione!";
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Categories = await categoryService.ReadAllAsync();
        return View(article);
    }
}