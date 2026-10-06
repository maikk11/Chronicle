using Chronicle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Controllers;

[Authorize(Roles = "Writer")]
public class WriterController : Controller
{
    private readonly ArticleService articleService;
    private readonly UserManager<IdentityUser> userManager;

    public WriterController(ArticleService articleService, UserManager<IdentityUser> userManager)
    {
        this.articleService = articleService;
        this.userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var articles = await articleService.ReadByUserAsync(user.Id);

        return View(articles);
    }
}