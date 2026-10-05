using Chronicle.Repositories;
using Chronicle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Controllers;

[Authorize(Roles = "Revisor")]
public class RevisorController : Controller
{
    private readonly IArticleRepository articleRepository;
    private readonly ArticleService articleService;

    public RevisorController(
        IArticleRepository articleRepository,
        ArticleService articleService)
    {
        this.articleRepository = articleRepository;
        this.articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var pending = (await articleService.ReadAllAsync())
            .Where(a => a.IsAccepted == null)
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        return View(pending);
    }
        [HttpGet]
    public async Task<IActionResult> Detail(long id)
    {
        var article = await articleService.ReadAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAccepted(long id, bool accepted)
    {
        var article = await articleRepository.GetAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        article.IsAccepted = accepted;

        if (accepted)
        {
            article.PublishDate = DateTime.UtcNow;
        }

        await articleRepository.UpdateAsync(article);

        TempData["SuccessMessage"] = accepted ? "Article accepted." : "Article rejected.";
        return RedirectToAction("Dashboard");
    }
}