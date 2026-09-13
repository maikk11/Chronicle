using Chronicle.Models.Domain;
using Chronicle.Data;
using Microsoft.EntityFrameworkCore;
namespace Chronicle.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly ChronicleDbContext _chronicleDbContext;
    public ArticleRepository(ChronicleDbContext chronicleDbContext)
    {
        this._chronicleDbContext = chronicleDbContext;
    }
    public async Task<IEnumerable<Article>> GetAllAsync()
    {
        return await _chronicleDbContext.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Image)
            .ToListAsync();
    }
    public async Task<Article?> GetAsync(long id)
    {
        return await _chronicleDbContext.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Image)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    public async Task<Article?> GetByTitleAsync(string title)
    {
        return await _chronicleDbContext.Articles.FirstOrDefaultAsync(x => x.Title == title);
    }
    public async Task<IEnumerable<Article>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<Article>();
        }

        var pattern = $"%{searchTerm}%";

        return await _chronicleDbContext.Articles
            .Include(x => x.User)
            .Include(x => x.Category)
            .Include(a => a.Image)
            .Where(a =>
                EF.Functions.Like(a.Title, pattern) ||
                EF.Functions.Like(a.Subtitle, pattern) ||
                (a.User != null && EF.Functions.Like(a.User.UserName, pattern)) ||
                (a.Category != null && EF.Functions.Like(a.Category.Name, pattern))
            )
            .ToListAsync();
    }
    public async Task<Article> AddAsync(Article article)
    {
        await _chronicleDbContext.Articles.AddAsync(article);
        await _chronicleDbContext.SaveChangesAsync();
        return article;
    }
    public async Task<Article?> UpdateAsync(Article article)
    {
        var existingArticle = await _chronicleDbContext.Articles.FindAsync(article.Id);

        if (existingArticle != null)
        {
            existingArticle.Title = article.Title;
            existingArticle.Subtitle = article.Subtitle;
            existingArticle.Body = article.Body;
            existingArticle.PublishDate = article.PublishDate;
            existingArticle.CategoryId = article.CategoryId;
            await _chronicleDbContext.SaveChangesAsync();
            return existingArticle;
        }
        return null;
    }
    public async Task<Article?> DeleteAsync(long id)
    {
        var existingArticle = await _chronicleDbContext.Articles.FindAsync(id);
        if (existingArticle != null)
        {
            _chronicleDbContext.Articles.Remove(existingArticle);
            await _chronicleDbContext.SaveChangesAsync();
            return existingArticle;
        }
        return null;
    }
    public async Task<IEnumerable<Article>> GetByCategoryAsync(long categoryId)
    {
        return await _chronicleDbContext.Articles
            .Where(a => a.CategoryId == categoryId)
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Image)
            .ToListAsync();
    }
    public async Task<IEnumerable<Article>> GetByUserAsync(string userId)
    {
        return await _chronicleDbContext.Articles
            .Where(a => a.UserId == userId)
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Image)
            .ToListAsync();
    }
}