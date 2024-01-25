using Moq;
using MSGraphApi.Downloader.Operations;
using MSGraphApi.Library.Services.GraphApi;

namespace MSGraphApi.Downloader.Tests;

public class GraphDownloaderTests
{
    [Fact]
    public async void Download_Should_Initialize_GraphApi()
    {
        var ctx = new TestContext();
        await ctx.Downloader.Download("MyOperation1");
        ctx.GraphApiMock.Verify(x => x.Init(It.IsAny<GraphApiSettings>()), Times.Once);
    }

    [Fact]
    public async void Download_Should_Download_With_Proper_Strategy()
    {
        var ctx = new TestContext();
        await ctx.Downloader.Download("MyOperation1");
        ctx.MyOpe1StrategyMock.Verify(o => o.FetchDataAsync(null), Times.Once);
    }

    [Fact]
    public async void Download_Should_Return_Summary_Result()
    {
        var ctx = new TestContext();
        ctx
            .MyOpe2StrategyMock.Setup(o => o.StoreDataAsync())
            .ReturnsAsync(
                new GraphDownloaderSummary()
                {
                    LocationPath = "path/to/my/jsons",
                    ObjectsCount = 121
                }
            );
        var summaryRes = await ctx.Downloader.Download("MyOperation2");
        Assert.Equal(121, summaryRes.ObjectsCount);
        Assert.Equal("path/to/my/jsons", summaryRes.LocationPath);
    }

    [Fact]
    public async void Download_Should_Store_Data_With_Proper_Strategy()
    {
        var ctx = new TestContext();
        await ctx.Downloader.Download("MyOperation1");
        ctx.MyOpe1StrategyMock.Verify(o => o.StoreDataAsync(), Times.Once);
    }

    [Fact]
    public async Task Download_Should_Throw_Operation_Not_Supported()
    {
        var ctx = new TestContext();

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await ctx.Downloader.Download("ThisOperationIsNotSupported")
        );

        Assert.Contains(
            "Operation \"ThisOperationIsNotSupported\" is not supported",
            exception.Message
        );

        ctx.MyOpe1StrategyMock.Verify(o => o.FetchDataAsync(null), Times.Never);
        ctx.MyOpe1StrategyMock.Verify(o => o.StoreDataAsync(), Times.Never);
    }

    [Fact]
    public async void Download_Should_Paginate_When_Pagination_Token_Is_Returned()
    {
        var ctx = new TestContext();
        ctx
            .MyOpe1StrategyMock.SetupSequence(o => o.FetchDataAsync(It.IsAny<string?>()))
            .ReturnsAsync("token1")
            .ReturnsAsync("token2")
            .ReturnsAsync((string?)null);
        await ctx.Downloader.Download("MyOperation1");

        ctx.MyOpe1StrategyMock.Verify(o => o.FetchDataAsync(null), Times.Once);
        ctx.MyOpe1StrategyMock.Verify(o => o.FetchDataAsync("token1"), Times.Once);
        ctx.MyOpe1StrategyMock.Verify(o => o.FetchDataAsync("token2"), Times.Once);
        ctx.MyOpe1StrategyMock.Verify(o => o.FetchDataAsync(It.IsAny<string?>()), Times.Exactly(3));

        ctx.MyOpe1StrategyMock.Verify(o => o.StoreDataAsync(), Times.Exactly(3));
    }

    [Fact]
    public async void Download_Should_Return_Summary_Result_With_Full_Object_Count_When_Paginating()
    {
        var ctx = new TestContext();
        ctx
            .MyOpe2StrategyMock.SetupSequence(o => o.FetchDataAsync(It.IsAny<string?>()))
            .ReturnsAsync("token1")
            .ReturnsAsync("token2")
            .ReturnsAsync((string?)null);
        ctx
            .MyOpe2StrategyMock.SetupSequence(o => o.StoreDataAsync())
            .ReturnsAsync(
                new GraphDownloaderSummary() { LocationPath = "path/to/jsons", ObjectsCount = 10 }
            )
            .ReturnsAsync(
                new GraphDownloaderSummary() { LocationPath = "path/to/jsons", ObjectsCount = 10 }
            )
            .ReturnsAsync(
                new GraphDownloaderSummary() { LocationPath = "path/to/jsons", ObjectsCount = 5 }
            );

        var summaryRes = await ctx.Downloader.Download("MyOperation2");
        Assert.Equal(25, summaryRes.ObjectsCount);
        Assert.Equal("path/to/jsons", summaryRes.LocationPath);
    }

    private class TestContext
    {
        public Mock<IOperationStrategy> MyOpe1StrategyMock { get; private set; }
        public Mock<IOperationStrategy> MyOpe2StrategyMock { get; private set; }
        public GraphDownloader Downloader { get; private set; }
        public Mock<IGraphApi> GraphApiMock { get; private set; }

        public TestContext()
        {
            GraphApiMock = new Mock<IGraphApi>();

            MyOpe1StrategyMock = new Mock<IOperationStrategy>();
            MyOpe1StrategyMock.Setup(x => x.Operation).Returns("MyOperation1");
            MyOpe2StrategyMock = new Mock<IOperationStrategy>();
            MyOpe2StrategyMock.Setup(x => x.Operation).Returns("MyOperation2");

            Downloader = new GraphDownloader(
                GraphApiMock.Object,
                new GraphApiSettings()
                {
                    ClientId = "client-1",
                    ClientSecret = "secret-1",
                    TenantId = "tenant-1"
                },
                [MyOpe1StrategyMock.Object, MyOpe2StrategyMock.Object]
            );
        }
    }
}
