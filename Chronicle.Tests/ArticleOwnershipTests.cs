using Chronicle.Models.ViewModels;
using Chronicle.Services;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Tests;

public class ArticleOwnershipTests
{
    private static IdentityUser UserWithId(string id) => new IdentityUser { Id = id };

    [Fact]
    public void IsOwnedBy_ReturnsTrue_WhenTheArticleBelongsToTheUser()
    {
        var user = UserWithId("writer-1");
        var article = new ArticleDto { User = user };

        Assert.True(ArticleOwnership.IsOwnedBy(article, user));
    }

    [Fact]
    public void IsOwnedBy_ReturnsFalse_WhenTheArticleBelongsToAnotherWriter()
    {
        var article = new ArticleDto { User = UserWithId("writer-1") };
        var otherWriter = UserWithId("writer-2");

        Assert.False(ArticleOwnership.IsOwnedBy(article, otherWriter));
    }

    [Fact]
    public void IsOwnedBy_ReturnsFalse_WhenTheArticleDoesNotExist()
    {
        Assert.False(ArticleOwnership.IsOwnedBy(null, UserWithId("writer-1")));
    }

    [Fact]
    public void IsOwnedBy_ReturnsFalse_WhenThereIsNoSignedInUser()
    {
        var article = new ArticleDto { User = UserWithId("writer-1") };

        Assert.False(ArticleOwnership.IsOwnedBy(article, null));
    }

    [Fact]
    public void IsOwnedBy_ReturnsFalse_WhenTheArticleHasNoAuthor()
    {
        var article = new ArticleDto { User = null };

        Assert.False(ArticleOwnership.IsOwnedBy(article, UserWithId("writer-1")));
    }
}