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
    private readonly IImageService imageService;
    public UserManager<IdentityUser> UserManager => userManager;

    public ArticleService(IArticleRepository articleRepository, UserManager<IdentityUser> userManager, IImageService imageService)
    {
        this.articleRepository = articleRepository;
        this.userManager = userManager;
        this.imageService = imageService;
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
        string? imageUrl = null;
        if (file != null && file.Length > 0)
        {
            imageUrl = await imageService.UploadAsync(file);
        }
        if (imageUrl != null)
        {
            await imageService.SaveToDbAsync(imageUrl, savedArticle.Id);
        }
        var finalArticle = await articleRepository.GetAsync(savedArticle.Id);
        return MapToDto(finalArticle!);
    }

    public async Task<List<ArticleDto>> ReadAllAsync()
    {
        var articles = await articleRepository.GetAllAsync();
        return articles.Select(MapToDto).ToList();
    }

    public async Task<ArticleDto?> ReadAsync(long key)
    {
        var article = await articleRepository.GetAsync(key);
        if (article == null)
        {
            return null;
        }
        return MapToDto(article);
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
        public async Task<List<ArticleDto>> SearchAsync(string searchTerm)
    {
        var articles = await articleRepository.SearchAsync(searchTerm);

        return articles
            .Where(a => a.IsAccepted == true)
            .OrderByDescending(a => a.PublishDate ?? a.CreatedAt)
            .Select(MapToDto)
            .ToList();
    }
     private static ArticleDto MapToDto(Article article)
    {
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
            Category = article.Category,
            Image = article.Image
        };
    }
}