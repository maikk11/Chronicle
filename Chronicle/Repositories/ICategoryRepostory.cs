using Chronicle.Models.Domain;
namespace Chronicle.Repositories;
public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetAsync(long id);
    Task<Category> AddAsync(Category category);
    Task<Category?> UpdateAsync(Category category);
    Task<Category?> DeleteAsync(long id);
}