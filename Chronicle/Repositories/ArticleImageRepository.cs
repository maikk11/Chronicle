using Chronicle.Data;
using Chronicle.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chronicle.Repositories;

public class ArticleImageRepository : IArticleImageRepository
{
    private readonly ChronicleDbContext _chronicleDbContext;
    public ArticleImageRepository(ChronicleDbContext chronicleDbContext)
    {
        this._chronicleDbContext = chronicleDbContext;
    }
    public async Task<Image> AddAsync(Image image)
    {
        await _chronicleDbContext.Images.AddAsync(image);
        await _chronicleDbContext.SaveChangesAsync();
        return image;
    }

    public async Task<Image?> DeleteByPathAsync(string path)
    {
        var existingImage = await _chronicleDbContext.Images.FirstOrDefaultAsync(x => x.Path == path);
        if(existingImage != null)
        {
             _chronicleDbContext.Images.Remove(existingImage);
            await _chronicleDbContext.SaveChangesAsync();
            return existingImage;
        }
        return null;
    }
}