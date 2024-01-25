namespace MSGraphApi.Downloader.Operations;

using System.Text.Json;
using Microsoft.Graph.Models;
using MSGraphApi.Library.Services.GraphApi;

public class GroupsOperationStrategy : BaseOperationStrategy
{
    private GroupCollectionResponse? _data;

    public GroupsOperationStrategy(
        IGraphApi graphApi,
        GraphApiSettings settings,
        IDataStorage dataStorage
    )
        : base(graphApi, settings, dataStorage) { }

    public override string Operation => "Download Groups";

    public override async Task<string?> FetchDataAsync(string? nextPageToken = null)
    {
        _data = await _graphApi.GetGroupsAsync(nextPageToken);
        return _data?.OdataNextLink;
    }

    public override async Task<GraphDownloaderSummary> StoreDataAsync()
    {
        var workingDir = $"{Directory.GetCurrentDirectory()}/MSGraph/Groups";
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
