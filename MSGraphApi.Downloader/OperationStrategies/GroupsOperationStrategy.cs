namespace MSGraphApi.Downloader.Operations;

using System.Text.Json;
using Microsoft.Graph.Models;
using MSGraphApi.Library.Services.GraphApi;

public class GroupsOperationStrategy : BaseOperationStrategy<List<Group>?>
{
    public GroupsOperationStrategy(
        IGraphApi graphApi,
        GraphApiSettings settings,
        IDataStorage dataStorage,
        IFileSystemHelpers fileSystemHelpers
    )
        : base(graphApi, settings, dataStorage, fileSystemHelpers) { }

    public override string Operation => "Download Groups";

    protected override async Task<OperationResponse<List<Group>?>> InvokeFetchDataAsync(
        string? nextPageToken
    )
    {
        var res = await _graphApi.GetGroupsAsync(nextPageToken);
        return new OperationResponse<List<Group>?>()
        {
            NextPageToken = res?.OdataNextLink,
            Data = res?.Value
        };
    }

    protected override string GetFormattedFilename(dynamic item)
    {
        return $"{item.DisplayName}-[{item.Id}]";
    }

    protected override string GetStorageFolder()
    {
        return "Groups";
    }
}
