using Chronicle.Models;

namespace Chronicle.Tests;

public class ErrorViewModelTests
{
    [Fact]
    public void ShowRequestId_IsTrue_WhenRequestIdIsSet()
    {
        var model = new ErrorViewModel { RequestId = "abc123" };

        Assert.True(model.ShowRequestId);
    }
}