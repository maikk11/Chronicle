using Chronicle.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Services;

public static class ArticleOwnership
{
    public static bool IsOwnedBy(ArticleDto? article, IdentityUser? user)
    {
        if (article == null || user == null)
        {
            return false;
        }

        return article.User?.Id == user.Id;
    }
}