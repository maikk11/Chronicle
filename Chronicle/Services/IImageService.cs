namespace Chronicle.Services;
public interface IImageService
{
    Task<string> UploadAsync(IFormFile file);
    Task SaveToDbAsync(string url, long articleId);
    Task DeleteAsync(string path);
}