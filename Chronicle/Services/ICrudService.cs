using System.Security.Claims;
namespace Chronicle.Services;
public interface ICrudService<TReadDto, TModel, TKey>
{
    Task<List<TReadDto>> ReadAllAsync();
    Task<TReadDto?> ReadAsync(TKey key);
    Task<TReadDto> CreateAsync(TModel model, ClaimsPrincipal principal, IFormFile? file);
    Task<TReadDto?> UpdateAsync(TKey key, TModel model, IFormFile? file);
    Task<bool> DeleteAsync(TKey key);
}