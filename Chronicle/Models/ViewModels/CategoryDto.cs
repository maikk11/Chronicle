namespace Chronicle.Models.ViewModels;
public class CategoryDto
{
    public long Id {get;set;}
    public string Name {get;set;} = String.Empty;
    public int NumberOfArticles {get;set;}
}