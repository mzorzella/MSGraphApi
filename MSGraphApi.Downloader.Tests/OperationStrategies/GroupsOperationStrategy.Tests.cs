using Moq;
using MSGraphApi.Library.Services.GraphApi;

namespace MSGraphApi.Downloader.Operations.Tests;

public class GroupsOperationStrategyTests
{
    [Fact]
    public async void FetchData_Should_Get_Groups()
    {
        var ctx = new TestContext();
        await ctx.Strategy.FetchDataAsync();
        ctx.GraphApiMock.Verify(x => x.GetGroupsAsync(null), Times.Once);
    }

    [Fact]
    public async void FetchData_With_Pagination_Token_Should_Get_Groups_With_Token()
    {
        var ctx = new TestContext();
        await ctx.Strategy.FetchDataAsync("token-1");
        ctx.GraphApiMock.Verify(x => x.GetGroupsAsync("token-1"), Times.Once);
    }

    [Fact(Skip = "TODO")]
    public void FetchData_Should_Return_Pagination_Token()
    {
        // TODO
    }

    private class TestContext
    {
        public GroupsOperationStrategy Strategy { get; private set; }
        public Mock<IGraphApi> GraphApiMock { get; private set; }

        public TestContext()
        {
            GraphApiMock = new Mock<IGraphApi>();

            Strategy = new GroupsOperationStrategy(
                GraphApiMock.Object,
                new GraphApiSettings()
                {
                    ClientId = "client-1",
                    ClientSecret = "secret-1",
                    TenantId = "tenant-1"
                }
            );
        }
    }
}
