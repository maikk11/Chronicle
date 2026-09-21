using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Chronicle.Models;

namespace Chronicle.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Services.ArticleService articleService;

    public HomeController(ILogger<HomeController> logger, Services.ArticleService articleService)
    {
        _logger = logger;
        this.articleService = articleService;
    }

    public async Task<IActionResult> Index()
    {
        var articles = await articleService.ReadAllAsync();

        var latestArticles = articles
            .OrderByDescending(a => a.PublishDate ?? a.CreatedAt)
            .Take(3)
            .ToList();

        return View(latestArticles);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}