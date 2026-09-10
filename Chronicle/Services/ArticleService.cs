using System.Security.Claims;
using Chronicle.Models.Domain;
using Chronicle.Models.ViewModels;
using Chronicle.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Services;

public class ArticleService : ICrudService<ArticleDto, Article, long>
{
    private readonly IArticleRepository articleRepository;
    private readonly UserManager<IdentityUser> userManager;

    public UserManager<IdentityUser> UserManager => userManager;

    public ArticleService(
        IArticleRepository articleRepository,
        UserManager<IdentityUser> userManager)
    {
        this.articleRepository = articleRepository;
        this.userManager = userManager;
    }

    public async Task<ArticleDto> CreateAsync(Article article, ClaimsPrincipal principal, IFormFile? file)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user != null)
        {
            article.UserId = user.Id;
        }
        article.IsAccepted = null;
        var savedArticle = await articleRepository.AddAsync(article);
        var finalArticle = await articleRepository.GetAsync(savedArticle.Id);
        return new ArticleDto
        {
            Id = finalArticle!.Id,
            Title = finalArticle.Title,
            Subtitle = finalArticle.Subtitle,
            Body = finalArticle.Body,
            PublishDate = finalArticle.PublishDate,
            CreatedAt = finalArticle.CreatedAt,
            User = finalArticle.User,
            Category = finalArticle.Category,
            IsAccepted = finalArticle.IsAccepted
        };
    }

    public async Task<List<ArticleDto>> ReadAllAsync()
    {
        var articles = await articleRepository.GetAllAsync();
        return articles.Select(a => new ArticleDto
        {
            Id = a.Id,
            Title = a.Title,
            Subtitle = a.Subtitle,
            Body = a.Body,
            PublishDate = a.PublishDate,
            CreatedAt = a.CreatedAt,
            IsAccepted = a.IsAccepted,
            User = a.User,
            Category = a.Category
        }).ToList();
    }

    public async Task<ArticleDto?> ReadAsync(long key)
    {
        var article = await articleRepository.GetAsync(key);
        if (article == null)
        {
            return null;
        }
        return new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Subtitle = article.Subtitle,
            Body = article.Body,
            CreatedAt = article.CreatedAt,
            PublishDate = article.PublishDate,
            IsAccepted = article.IsAccepted,
            User = article.User,
            Category = article.Category
        };
    }

    public async Task<ArticleDto?> UpdateAsync(long key, Article model, IFormFile? file)
    {
        model.Id = key;
        var article = await articleRepository.UpdateAsync(model);
        if (article == null)
        {
            return null;
        }
        return await ReadAsync(key);
    }

    public async Task<bool> DeleteAsync(long key)
    {
        var article = await articleRepository.DeleteAsync(key);
        return article != null;
    }
}