using System.Text.Json;
using Moq;
using MSGraphApi.Library.Services.GraphApi;

namespace MSGraphApi.Downloader.Operations.Tests;

public class BaseOperationStrategyTests
{
    [Fact]
    public async void FetchData_Should_Invoke_Fetch_Data_On_Concrete_Class()
    {
        var ctx = new TestContext();
        await ctx.Strategy.FetchDataAsync();
        Assert.True(ctx.Strategy.InvokeFetchDataAsyncIsCalled);
    }

    [Fact]
    public async void FetchData_Should_Wrap_Exception_When_Fetch_Fails()
    {
        var ctx = new TestContext();
        ctx
            .GraphApiMock.Setup(ga => ga.GetGroupsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Graph API error!"));

        var exception = await Assert.ThrowsAsync<GraphApiException>(
            async () => await ctx.Strategy.FetchDataAsync()
        );

        Assert.Contains("Cannot fetch Graph API data", exception.Message);
    }

    [Fact]
    public async void StoreData_With_Collection_Of_Item_Should_Store_Each_Item_In_Proper_Folder()
    {
        var ctx = new TestContext();
        await ctx.Strategy.FetchDataAsync();

        await ctx.Strategy.StoreDataAsync();
        ctx.DataStorage.Verify(ds =>
            ds.StoreData(
                $"{TestContext.CurrentDir}/MSGraph/{TestStrategyWithList.MyListFolder}/name-1###id-1.json",
                JsonSerializer.Serialize(
                    (object)TestStrategyWithList.FakeData.ElementAt(0),
                    new JsonSerializerOptions() { WriteIndented = true }
                )
            )
        );
        ctx.DataStorage.Verify(ds =>
            ds.StoreData(
                $"{TestContext.CurrentDir}/MSGraph/{TestStrategyWithList.MyListFolder}/name-2###id-2.json",
                JsonSerializer.Serialize(
                    (object)TestStrategyWithList.FakeData.ElementAt(1),
                    new JsonSerializerOptions() { WriteIndented = true }
                )
            )
        );
    }

    [Fact]
    public async void StoreData_Should_Return_Summary_Info()
    {
        var ctx = new TestContext();
        await ctx.Strategy.FetchDataAsync();

        var summary = await ctx.Strategy.StoreDataAsync();

        Assert.Equal(2, summary.ObjectsCount);
        Assert.Equal(
            $"{TestContext.CurrentDir}/MSGraph/{TestStrategyWithList.MyListFolder}",
            summary.LocationPath
        );
    }

    [Fact]
    public async void StoreData_Should_Wrap_Exception_When_Store_Fails()
    {
        var ctx = new TestContext();
        ctx
            .DataStorage.Setup(ds => ds.StoreData(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Error storing data!"));

        await ctx.Strategy.FetchDataAsync();

        var exception = await Assert.ThrowsAsync<GraphApiDataStorageException>(async () =>
        {
            await ctx.Strategy.StoreDataAsync();
        });

        Assert.Contains("Cannot store Graph API data", exception.Message);
    }

    private class TestContext
    {
        public const string CurrentDir = "current-dir";
        public TestStrategyWithList Strategy { get; private set; }
        public Mock<IGraphApi> GraphApiMock { get; private set; }
        public Mock<IDataStorage> DataStorage { get; private set; }
        public Mock<IFileSystemHelpers> FileSystemHelper { get; private set; }

        public TestContext()
        {
            GraphApiMock = new Mock<IGraphApi>();
            DataStorage = new Mock<IDataStorage>();
            FileSystemHelper = new Mock<IFileSystemHelpers>();
            FileSystemHelper.Setup(h => h.GetCurrentDirectory()).Returns(CurrentDir);

            Strategy = new TestStrategyWithList(
                GraphApiMock.Object,
                new GraphApiSettings()
                {
                    ClientId = "client-1",
                    ClientSecret = "secret-1",
                    TenantId = "tenant-1"
                },
                DataStorage.Object,
                FileSystemHelper.Object
            );
        }
    }

    private class TestStrategyWithList : BaseOperationStrategy<List<dynamic>>
    {
        public const string MyListFolder = "mylistfolder";

        public static List<dynamic> FakeData = new List<dynamic>()
        {
            new { Name = "name-1", Id = "id-1" },
            new { Name = "name-2", Id = "id-2" }
        };

        public TestStrategyWithList(
            IGraphApi graphApi,
            GraphApiSettings settings,
            IDataStorage dataStorage,
            IFileSystemHelpers fileSystemHelpers
        )
            : base(graphApi, settings, dataStorage, fileSystemHelpers) { }

        public override string Operation => "List Strategy";

        public bool InvokeFetchDataAsyncIsCalled { get; private set; }

        protected override string GetFormattedFilename(dynamic item)
        {
            return $"{item.Name}###{item.Id}";
        }

        protected override string GetStorageFolder()
        {
            return MyListFolder;
        }

        protected override async Task<OperationResponse<List<dynamic>?>> InvokeFetchDataAsync(
            string? nextPageToken
        )
        {
            InvokeFetchDataAsyncIsCalled = true;

            var res = await _graphApi.GetGroupsAsync();
            return await Task.FromResult(
                new OperationResponse<List<dynamic>?>() { NextPageToken = null, Data = FakeData }
            );
        }
    }
}
