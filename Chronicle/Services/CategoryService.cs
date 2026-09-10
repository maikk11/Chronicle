using System.Security.Claims;
using Chronicle.Models.Domain;
using Chronicle.Models.ViewModels;
using Chronicle.Repositories;

namespace Chronicle.Services;
public class CategoryService : ICrudService<CategoryDto, Category, long>
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }
    public async Task<List<CategoryDto>> ReadAllAsync()
    {
        var categories = await categoryRepository.GetAllAsync();

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            NumberOfArticles = c.Articles?.Count ?? 0
        }).ToList();
    }
    public async Task<CategoryDto?> ReadAsync(long key)
    {
        var category = await categoryRepository.GetAsync(key);

        if (category == null)
        {
            return null;
        }
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NumberOfArticles = category.Articles?.Count ?? 0
        };
    }
    public async Task<CategoryDto> CreateAsync(Category model, ClaimsPrincipal principal, IFormFile? file)
    {
        var category = await categoryRepository.AddAsync(model);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NumberOfArticles = category.Articles?.Count ?? 0
        };
    }
    public async Task<CategoryDto?> UpdateAsync(long key, Category model, IFormFile? file)
    {
        model.Id = key;
        var category = await categoryRepository.UpdateAsync(model);
        if (category == null)
        {
            return null;
        }
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NumberOfArticles = category.Articles?.Count ?? 0
        };
    }
    public async Task<bool> DeleteAsync(long key)
    {
        var category = await categoryRepository.DeleteAsync(key);
        return category != null;
    }
}