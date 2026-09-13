using Chronicle.Models.Domain;
namespace Chronicle.Repositories;
public interface IArticleImageRepository
{
    Task<Image> AddAsync(Image image);
    Task<Image?> DeleteByPathAsync(string path);
}