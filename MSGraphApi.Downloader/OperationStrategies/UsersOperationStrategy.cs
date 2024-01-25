namespace MSGraphApi.Downloader.Operations;

using System.Text.Json;
using Microsoft.Graph.Models;
using MSGraphApi.Library.Services.GraphApi;

public class UsersOperationStrategy : BaseOperationStrategy
{
    private UserCollectionResponse? _data;

    public UsersOperationStrategy(
        IGraphApi graphApi,
        GraphApiSettings settings,
        IDataStorage dataStorage
    )
        : base(graphApi, settings, dataStorage) { }

    public override string Operation => "Download Users";

    public override async Task<string?> FetchDataAsync(string? nextPageToken)
    {
        _data = await _graphApi.GetUsersAsync(nextPageToken);
        return _data?.OdataNextLink;
    }

    public override async Task<GraphDownloaderSummary> StoreDataAsync()
    {
        var workingDir = $"{Directory.GetCurrentDirectory()}/MSGraph/Users";
        var tasks = new List<Task>();
        _data?.Value?.ForEach(g =>
            tasks.Add(
                _dataStorage.Store(
                    $"{workingDir}/{g.DisplayName}-[{g.Id}].json",
                    JsonSerializer.Serialize(
                        g,
                        new JsonSerializerOptions() { WriteIndented = true }
                    )
                )
            )
        );
        await Task.WhenAll(tasks);

        return new GraphDownloaderSummary()
        {
            LocationPath = workingDir,
            ObjectsCount = _data?.Value?.Count ?? 0
        };
    }
}
