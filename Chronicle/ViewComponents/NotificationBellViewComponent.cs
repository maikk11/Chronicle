using Chronicle.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.ViewComponents;

public class NotificationBellViewComponent : ViewComponent
{
    private readonly ICareerRequestRepository careerRequestRepository;
    private readonly IArticleRepository articleRepository;

    public NotificationBellViewComponent(
        ICareerRequestRepository careerRequestRepository,
        IArticleRepository articleRepository)
    {
        this.careerRequestRepository = careerRequestRepository;
        this.articleRepository = articleRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync(string role)
    {
        var count = 0;

        if (role == "Admin")
        {
            var pendingRequests = await careerRequestRepository.FindByIsCheckedAsync(false);
            count = pendingRequests.Count();
        }
        else if (role == "Revisor")
        {
            var articles = await articleRepository.GetAllAsync();
            count = articles.Count(a => a.IsAccepted == null);
        }

        return View(count);
    }
}