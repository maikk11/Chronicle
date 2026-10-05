using Chronicle.Models.Domain;
using Chronicle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Controllers;

[Authorize]
public class OperationsController : Controller
{
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<IdentityUser> userManager;
    private readonly ICareerRequestService careerRequestService;

    public OperationsController(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager,
        ICareerRequestService careerRequestService)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        this.careerRequestService = careerRequestService;
    }

    private async Task<List<IdentityRole>> GetAvailableRolesAsync(string userId)
    {
        var requestableRoles = roleManager.Roles
            .Where(r => r.Name != "Admin")
            .ToList();

        var availableRoles = new List<IdentityRole>();

        foreach (var role in requestableRoles)
        {
            var alreadyAssigned = await careerRequestService.IsRoleAlreadyAssignedAsync(userId, role.Id);
            var alreadyPending = await careerRequestService.IsRequestPendingAsync(userId, role.Id);

            if (!alreadyAssigned && !alreadyPending)
            {
                availableRoles.Add(role);
            }
        }

        return availableRoles;
    }

    [HttpGet]
    public async Task<IActionResult> CareerRequest()
    {
        var userId = userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var availableRoles = await GetAvailableRolesAsync(userId);

        if (availableRoles.Count == 0)
        {
            TempData["ErrorMessage"] = "There are no roles you can apply for right now.";
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Roles = availableRoles;
        return View(new CareerRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CareerRequestStore(CareerRequest careerRequest)
    {
        var userId = userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(careerRequest.RoleId))
        {
            ModelState.AddModelError("RoleId", "Select a valid role.");
        }
        else
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole != null && careerRequest.RoleId == adminRole.Id)
            {
                ModelState.AddModelError("RoleId", "Select a valid role.");
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await GetAvailableRolesAsync(userId);
            return View("CareerRequest", careerRequest);
        }

        if (await careerRequestService.IsRoleAlreadyAssignedAsync(userId, careerRequest.RoleId))
        {
            TempData["ErrorMessage"] = "You already have that role.";
            return RedirectToAction("Index", "Home");
        }

        if (await careerRequestService.IsRequestPendingAsync(userId, careerRequest.RoleId))
        {
            TempData["ErrorMessage"] = "You already have a pending application for that role.";
            return RedirectToAction("Index", "Home");
        }

        careerRequest.UserId = userId;
        await careerRequestService.SaveAsync(careerRequest);

        TempData["SuccessMessage"] = "Application sent.";
        return RedirectToAction("Index", "Home");
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> CareerDetail(long id)
    {
        var request = await careerRequestService.FindAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        return View(request);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CareerAccept(long id)
    {
        await careerRequestService.AcceptAsync(id);
        TempData["SuccessMessage"] = "Role granted to the applicant.";
        return RedirectToAction("Dashboard", "Admin");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CareerReject(long id)
    {
        await careerRequestService.RejectAsync(id);
        TempData["SuccessMessage"] = "Application rejected.";
        return RedirectToAction("Dashboard", "Admin");
    }
}