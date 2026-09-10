using Chronicle.Models.Domain;
namespace Chronicle.Repositories;
public interface IArticleRepository
{
   Task<IEnumerable<Article>> GetAllAsync();
   Task<Article?> GetAsync(long id);
   Task<Article?> GetByTitleAsync(string title);
   Task<IEnumerable<Article>> SearchAsync(string searchTerm);
   Task<Article> AddAsync(Article article);
   Task<Article?> UpdateAsync(Article article);
   Task<Article?> DeleteAsync(long id);
   Task<IEnumerable<Article>> GetByCategoryAsync(long categoryId);
   Task<IEnumerable<Article>> GetByUserAsync(string userId);
}