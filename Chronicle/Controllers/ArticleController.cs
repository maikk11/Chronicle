using Microsoft.AspNetCore.Mvc;
using Chronicle.Models.Domain;
using Chronicle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Controllers;
public class ArticleController : Controller
{
    private readonly CategoryService categoryService;
    private readonly ArticleService articleService;
    private readonly UserManager<IdentityUser> userManager;

    public ArticleController(CategoryService categoryService, ArticleService articleService, UserManager<IdentityUser> userManager)
    {
        this.categoryService = categoryService;
        this.articleService = articleService;
        this.userManager = userManager;
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
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return RedirectToAction("Index");
        }

        var articles = await articleService.SearchAsync(keyword);

        ViewBag.Title = $"Search results for: {keyword}";
        return View("Index", articles);
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
    [ValidateAntiForgeryToken]
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
        if(article==null || article.IsAccepted != true)
        {
            return NotFound();
        }
        return View(article);
    }
    [Authorize(Roles = "Writer")]
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var articleDto = await articleService.ReadAsync(id);
        if (articleDto == null)
        {
            return NotFound();
        }

        var user = await userManager.GetUserAsync(User);
        if (!ArticleOwnership.IsOwnedBy(articleDto, user))
        {
            return Forbid();
        }

        var article = new Article
        {
            Id = articleDto.Id,
            Title = articleDto.Title,
            Subtitle = articleDto.Subtitle,
            Body = articleDto.Body,
            CategoryId = articleDto.Category?.Id,
            Image = articleDto.Image
        };

        ViewBag.Categories = await categoryService.ReadAllAsync();

        return View(article);
    }
    [Authorize(Roles = "Writer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(long id, Article article, IFormFile? file)
    {
        var existingArticle = await articleService.ReadAsync(id);
        if (existingArticle == null)
        {
            return NotFound();
        }

        var user = await userManager.GetUserAsync(User);
        if (!ArticleOwnership.IsOwnedBy(existingArticle, user))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await categoryService.ReadAllAsync();
            return View("Edit", article);
        }

        await articleService.UpdateAsync(id, article, file);

        TempData["SuccessMessage"] = "Article updated and sent back for review.";
        return RedirectToAction("Dashboard", "Writer");
    }

    [Authorize(Roles = "Writer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        var existingArticle = await articleService.ReadAsync(id);
        if (existingArticle == null)
        {
            return NotFound();
        }
        var user = await userManager.GetUserAsync(User);
        if (!ArticleOwnership.IsOwnedBy(existingArticle, user))
        {
            return Forbid();
        }
        await articleService.DeleteAsync(id);
        TempData["SuccessMessage"] = "Article deleted.";
        return RedirectToAction("Dashboard", "Writer");
    }
}