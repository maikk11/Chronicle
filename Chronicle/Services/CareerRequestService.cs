using Chonicle.Services;
using Chronicle.Models.Domain;
using Chronicle.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Services;

public class CareerRequestService : ICareerRequestService
{
    private readonly ICareerRequestRepository careerRequestRepository;
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IEmailService emailService;
    private readonly IConfiguration configuration;
    public CareerRequestService(ICareerRequestRepository careerRequestRepository, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IConfiguration configuration)
    {
        this.careerRequestRepository = careerRequestRepository;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.emailService = emailService;
        this.configuration = configuration;
    }

    public async Task AcceptAsync(long requestId)
    {
        var request = await careerRequestRepository.GetAsync(requestId);
        if(request == null)
        {
            return;
        }
        var user = await userManager.FindByIdAsync(request.UserId);
        var role = await roleManager.FindByIdAsync(request.RoleId);
        if (user == null || role == null)
        {
            return;
        }

        await userManager.AddToRoleAsync(user, role.Name!);

        request.IsChecked = true;
        request.IsApproved = true;
        await careerRequestRepository.UpdateAsync(request);

        await emailService.SendEmailAsync(
            user.Email!,
            "Your application has been accepted",
            $"<p>You have been given the {role.Name} role on Chronicle.</p>");
    }

    public async Task<CareerRequest?> FindAsync(long id)
    {
        return await careerRequestRepository.GetAsync(id);
    }

    public async Task<bool> IsRequestPendingAsync(string userId, string roleId)
    {
        var requests = await careerRequestRepository.GetByUserIdAsync(userId);
        var hasPending = requests.Any(r => r.RoleId == roleId && !r.IsChecked);
        return hasPending;
    }

    public async Task<bool> IsRoleAlreadyAssignedAsync(string userId, string roleId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if(user == null)
        {
            return false;
        }
        var role = await roleManager.FindByIdAsync(roleId);
        if(role == null)
        {
            return false;
        }
        return await userManager.IsInRoleAsync(user, role.Name!);
    }

    public async Task RejectAsync(long requestId)
    {
        var request = await careerRequestRepository.GetAsync(requestId);
        if (request == null)
        {
            return;
        }

        var user = await userManager.FindByIdAsync(request.UserId);
        var role = await roleManager.FindByIdAsync(request.RoleId);

        if (user == null || role == null)
        {
            return;
        }

        request.IsChecked = true;
        request.IsApproved = false;
        await careerRequestRepository.UpdateAsync(request);

        await emailService.SendEmailAsync(
            user.Email!,
            "Your application was not accepted",
            $"<p>Your application for the {role.Name} role was not accepted this time.</p>");
    }

    public async Task SaveAsync(CareerRequest careerRequest)
    {
        careerRequest.IsChecked = false;
        careerRequest.IsApproved = null;
        await careerRequestRepository.AddAsync(careerRequest);
        var user = await userManager.FindByIdAsync(careerRequest.UserId);
        var role = await roleManager.FindByIdAsync(careerRequest.RoleId);
        var adminAddress = configuration["Email:AdminAddress"]
        ?? throw new InvalidOperationException("Email:AdminAddress is not configured.");
        await emailService.SendEmailAsync(adminAddress,"New role request",
        $"<p>{user?.UserName} has applied for the {role?.Name} role.</p>" +
        $"<p>{careerRequest.Body}</p>");
    }
}