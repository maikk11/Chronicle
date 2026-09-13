using Chronicle.Repositories;
using Chronicle.Models.Domain;
using System.Net.Http.Headers;

namespace Chronicle.Services;

public class SupabaseImageService : IImageService
{
    private readonly IConfiguration configuration;
    private readonly IArticleImageRepository imageRepository;
    private readonly HttpClient httpClient;
    public SupabaseImageService(IConfiguration configuration, IArticleImageRepository imageRepository, HttpClient httpClient)
    {
        this.configuration = configuration;
        this.imageRepository = imageRepository;
        this.httpClient = httpClient;
    }
    public async Task<string> UploadAsync(IFormFile file)
    {
        var supabaseUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url is not configured.");
        var supabaseKey = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key is not configured.");
        var supabaseBucket = configuration["Supabase:Bucket"] ?? throw new InvalidOperationException("Supabase:Bucket is not configured.");

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var uploadUrl = $"{supabaseUrl}{supabaseBucket}{fileName}";

        using var content = new StreamContent(file.OpenReadStream());
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supabaseKey);
        httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);

        var response = await httpClient.PostAsync(uploadUrl, content);

        if (response.IsSuccessStatusCode)
        {
            return uploadUrl;
        }

        throw new Exception($"Failed to upload image to Supabase: {response.ReasonPhrase}");
    }
    public async Task SaveToDbAsync(string url, long articleId)
    {
        var publicUrlPrefix = configuration["Supabase:PublicUrl"] ?? throw new InvalidOperationException("Supabase:PublicUrl is not configured.");
        var bucketPrefix = configuration["Supabase:Bucket"] ?? throw new InvalidOperationException("Supabase:Bucket is not configured.");
        var publicUrl = url.Replace(bucketPrefix, publicUrlPrefix);
        await imageRepository.AddAsync(new Image
        {
           Path = publicUrl,
           ArticleId = articleId
        });
    }
    public async Task DeleteAsync(string path)
    {
        var supabaseKey = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key is not configured.");
        var publicUrlPrefix = configuration["Supabase:PublicUrl"] ?? throw new InvalidOperationException("Supabase:PublicUrl is not configured.");
        var bucketPrefix = configuration["Supabase:Bucket"] ?? throw new InvalidOperationException("Supabase:Bucket is not configured.");
        var deleteUrl = path.Replace(publicUrlPrefix, bucketPrefix);
       httpClient.DefaultRequestHeaders.Clear();
       httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supabaseKey);
       httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
       var response = await httpClient.DeleteAsync(deleteUrl);
        if (response.IsSuccessStatusCode)
        {
            await imageRepository.DeleteByPathAsync(path);
        }
    }
}