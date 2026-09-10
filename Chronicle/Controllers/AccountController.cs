using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Chronicle.Models.ViewModels;

namespace Chronicle.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.FindByEmailAsync(model.Username);
        var userNameToSign = user?.UserName ?? model.Username;

        var result = await signInManager.PasswordSignInAsync(
            userNameToSign,
            model.Password,
            false,
            false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid username or password");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var createResult = await userManager.CreateAsync(user, model.Password);

        if (createResult.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
            TempData["SuccessMessage"] = "Registration complete!";
            return RedirectToAction("Index", "Home");
        }
        foreach (var error in createResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}