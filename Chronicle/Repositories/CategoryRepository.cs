using Chronicle.Data;
using Chronicle.Models.Domain;
using Microsoft.EntityFrameworkCore;
namespace Chronicle.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ChronicleDbContext _chronicleDbContext;
    public CategoryRepository(ChronicleDbContext chronicleDbContext)
    {
        this._chronicleDbContext = chronicleDbContext;
    }
    public async Task<IEnumerable<Category>> GetAllAsync() => await _chronicleDbContext.Categories.OrderBy(c=>c.Name).ToListAsync();
    public async Task<Category?> GetAsync(long id)
    {
        return await _chronicleDbContext.Categories
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<Category> AddAsync(Category category)
    {
        await _chronicleDbContext.Categories.AddAsync(category);
        await _chronicleDbContext.SaveChangesAsync();
        return category;
    }
    public async Task<Category?> UpdateAsync(Category category)
    {
        var existingCategory = await _chronicleDbContext.Categories.FindAsync(category.Id);

        if (existingCategory != null)
        {
            existingCategory.Name = category.Name;
            await _chronicleDbContext.SaveChangesAsync();
            return existingCategory;
        }
        return null;
    }
    public async Task<Category?> DeleteAsync(long id)
    {
        var existingCategory = await _chronicleDbContext.Categories.FindAsync(id);

        if (existingCategory != null)
        {
            _chronicleDbContext.Categories.Remove(existingCategory);
            await _chronicleDbContext.SaveChangesAsync();
            return existingCategory;
        }
        return null;
    }
}